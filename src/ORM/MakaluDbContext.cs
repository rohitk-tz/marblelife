using Core;
using Core.Application;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Sales.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Core.Notification.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Core.Reports.Domain;
using Core.Review.Domain;
using Core.MarketingLead.Domain;
using Core.Scheduler.Domain;
using Core.ToDo.Domain;
using Core.Users.Impl;

namespace ORM
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class MakaluDbContext : DbContext
    {
        private static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        public MakaluDbContext() : base(DbConnection.DbContextConnectionAttribute)
        {
        }

        public override int SaveChanges()
        {
            var changeSet = ChangeTracker.Entries();
            var clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            var sessionContext = ApplicationManager.DependencyInjection.Resolve<ISessionContext>();
            long? currentUser = sessionContext != null && sessionContext.UserSession != null
                                            ? sessionContext.UserSession.OrganizationRoleUserId
                                            : 1;

            if (changeSet != null)
            {
                SetDataRecorderMetaData(changeSet, clock, currentUser);
                changeSet = ChangeTracker.Entries();
            }

            foreach (var entry in changeSet.Where(p => p.State == EntityState.Deleted))
            {
                SoftDelete(entry);
            }

            //var unchanged = changeSet.Where(x => x.State != EntityState.Unchanged).ToList();

            //if (changeSet.All(x => x.State == EntityState.Unchanged)) //In case all are unchanged
            //{
            //    return 0;
            //}

            return base.SaveChanges();
        }

        private static void SetDataRecorderMetaData(IEnumerable<DbEntityEntry> changeSet, IClock clock, long? currentUser)
        {
            foreach (var entry in changeSet.Where(c => c.State != EntityState.Unchanged))
            {
                foreach (var item in entry.Entity.GetType().GetProperties().Where(x => x.PropertyType == typeof(DataRecorderMetaData)).ToArray())
                {
                    var domainBase = entry.Entity as DomainBase;
                    if (domainBase == null) continue;

                    var metaProperty = entry.Entity.GetType().GetProperty("DataRecorderMetaData");
                    if (metaProperty.GetValue(entry.Entity) == null) continue;

                    if (domainBase.IsNew)
                    {
                        metaProperty.SetValue(entry.Entity,
                            new DataRecorderMetaData()
                            {
                                Id = 0,
                                IsNew = true,
                                DateCreated = clock.UtcNow,
                                CreatedBy = currentUser

                            });
                    }
                    else
                    {
                        var dr = (DataRecorderMetaData)metaProperty.GetValue(entry.Entity, null);
                        dr.DateModified = clock.UtcNow;
                        dr.ModifiedBy = currentUser;
                        metaProperty.SetValue(entry.Entity, dr);
                    }
                }

            }
        }

        private EntityTypeConfiguration<T> HandleDelete<T>(EntityTypeConfiguration<T> arg) where T : DomainBase
        {
            return arg.Map(m => m.Requires("IsDeleted").HasValue(false)).Ignore(m => m.IsDeleted);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<City>().HasMany(m => m.Zips).WithMany(m => m.Cities).Map(m =>
            {
                m.MapLeftKey("CityId");
                m.MapRightKey("ZipId");
                m.ToTable("CityZip");
            });

            modelBuilder.Entity<Organization>().HasMany(m => m.Address).WithMany(m => m.Organizations).Map(m =>
            {
                m.MapLeftKey("OrganizationId");
                m.MapRightKey("AddressId");
                m.ToTable("OrganizationAddress");
            });


            modelBuilder.Entity<Person>().HasMany(m => m.Addresses).WithMany(m => m.Persons).Map(m =>
            {
                m.MapLeftKey("PersonId");
                m.MapRightKey("AddressId");
                m.ToTable("PersonAddress");
            });

            modelBuilder.Entity<Person>().HasMany(m => m.Phones).WithMany(m => m.Persons).Map(m =>
            {
                m.MapLeftKey("PersonId");
                m.MapRightKey("PhoneId");
                m.ToTable("PersonPhone");
            });

            modelBuilder.Entity<Organization>().HasMany(m => m.Phones).WithMany(m => m.Organizations).Map(m =>
            {
                m.MapLeftKey("OrganizationId");
                m.MapRightKey("PhoneId");
                m.ToTable("OrganizationPhone");
            });


            modelBuilder.Entity<NotificationEmail>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<NotificationEmail>().HasRequired(x => x.NotificationQueue).WithRequiredDependent(x => x.NotificationEmail);
            modelBuilder.Entity<NotificationResource>().HasRequired(x => x.NotificationEmail).WithMany(x => x.Resources);

            // ----------------- Handle IsDeleted -------------

            var allEntityTypes = typeof(Person).Assembly.GetTypes().Where(x => x.BaseType == typeof(DomainBase)).ToList();

            foreach (var item in allEntityTypes)
            {
                var method = modelBuilder.GetType().GetMethod("Entity");

                HandleDelete(method.MakeGenericMethod(item).Invoke(modelBuilder, null) as dynamic);
            }

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        private void SoftDelete(DbEntityEntry entry)
        {
            Type entryEntityType = entry.Entity.GetType();

            string tableName = GetTableName(entryEntityType);
            var primaryKeyArray = GetPrimaryKeyName(entryEntityType);
            string sql = string.Format("UPDATE {0} SET IsDeleted = 1 WHERE", tableName);

            for (int index = 0; index < primaryKeyArray.Count; index++)
            {
                sql += " " + primaryKeyArray[index].Name + "=" + entry.OriginalValues[primaryKeyArray[index].Name] + " ";

                if (index < primaryKeyArray.Count - 1)
                {
                    sql = sql + " and ";
                }
            }

            Database.ExecuteSqlCommand(sql);
            // prevent hard delete            
            entry.State = EntityState.Detached;
        }

        private string GetTableName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            //return string.Format("[{0}].[{1}]",
            //    es.MetadataProperties["Schema"].Value,
            //    es.MetadataProperties["Table"].Value);
            return string.Format("{0}",
               es.MetadataProperties["Table"].Value);
        }

        private ReadOnlyMetadataCollection<EdmMember> GetPrimaryKeyName(Type type)
        {
            EntitySetBase es = GetEntitySet(type);

            return es.ElementType.KeyMembers;
        }

        private EntitySetBase GetEntitySet(Type type)
        {
            if (!_mappingCache.ContainsKey(type))
            {
                ObjectContext octx = ((IObjectContextAdapter)this).ObjectContext;

                string typeName = ObjectContext.GetObjectType(type).Name;

                var es = octx.MetadataWorkspace
                                .GetItemCollection(DataSpace.SSpace)
                                .GetItems<EntityContainer>()
                                .SelectMany(c => c.BaseEntitySets
                                                .Where(e => e.Name == typeName))
                                .FirstOrDefault();

                if (es == null)
                    throw new ArgumentException("Entity type not found in GetTableName", typeName);

                _mappingCache.Add(type, es);
            }

            return _mappingCache[type];
        }

        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserLog> UserLog { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<OrganizationRoleUser> OrganizationRoleUser { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<DataRecorderMetaData> DataRecorderMetaData { get; set; }
        public virtual DbSet<Lookup> Lookup { get; set; }
        public virtual DbSet<LookupType> LookupType { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Zip> Zip { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Franchisee> Franchisee { get; set; }
        public virtual DbSet<FeeProfile> FeeProfile { get; set; }
        public virtual DbSet<LateFee> LateFee { get; set; }
        public virtual DbSet<RoyaltyFeeSlabs> RoyaltyFeeSlabs { get; set; }
        public virtual DbSet<FranchiseeService> FranchiseeService { get; set; }
        public virtual DbSet<FranchiseeSales> FranchiseeSales { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }

        public virtual DbSet<ChargeCard> ChargeCard { get; set; }
        public virtual DbSet<ChargeCardPayment> ChargeCardPayment { get; set; }
        public virtual DbSet<ECheck> ECheck { get; set; }
        public virtual DbSet<ECheckPayment> ECheckPayment { get; set; }
        public virtual DbSet<FranchiseeInvoice> FranchiseeInvoice { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItem { get; set; }
        public virtual DbSet<LateFeeInvoiceItem> LateFeeInvoiceItem { get; set; }
        public virtual DbSet<InterestRateInvoiceItem> InterestRateInvoiceItem { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentItem> PaymentItem { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerEmail> CustomerEmail { get; set; }
        public virtual DbSet<MarketingClass> CustomerClass { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<SalesDataUpload> SalesDataUpload { get; set; }
        public virtual DbSet<NotificationType> NotificationType { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<NotificationQueue> NotificationQueue { get; set; }
        public virtual DbSet<NotificationEmail> NotificationEmail { get; set; }
        public virtual DbSet<NotificationResource> NotificationResource { get; set; }
        public virtual DbSet<NotificationEmailRecipient> NotificationEmailRecipient { get; set; }
        public virtual DbSet<SalesRep> SalesRep { get; set; }
        public virtual DbSet<FranchiseePaymentProfile> FranchiseeAuthNetProfile { get; set; }
        public virtual DbSet<PaymentInstrument> PaymentInstrument { get; set; }
        public virtual DbSet<Check> Check { get; set; }
        public virtual DbSet<InvoicePayment> InvoicePayment { get; set; }
        public virtual DbSet<CheckPayment> CheckPayment { get; set; }
        public virtual DbSet<CustomerFileUpload> CustomerDataUpload { get; set; }
        public virtual DbSet<PaymentMailReminder> PaymentMailReminder { get; set; }
        public virtual DbSet<SalesDataMailReminder> SalesDataMailReminder { get; set; }
        public virtual DbSet<AuthorizeNetApiMaster> AuthorizeNetApiMaster { get; set; }
        public virtual DbSet<FranchiseeSalesPayment> FranchiseeSalesPayment { get; set; }
        public virtual DbSet<FranchiseeAccountCredit> FranchiseeAccountCredit { get; set; }
        public virtual DbSet<AccountCreditPayment> AccountCreditPayment { get; set; }
        public virtual DbSet<FranchiseeNotes> FranchiseeNotes { get; set; }
        public virtual DbSet<WeeklyNotification> WeeklyNotification { get; set; }
        public virtual DbSet<CustomerFeedbackResponse> CustomerFeedbackResponse { get; set; }
        public virtual DbSet<CustomerReviewSystemRecord> CustomerReviewSystemRecord { get; set; }
        public virtual DbSet<CustomerFeedbackRequest> CustomerFeedbackRequest { get; set; }
        public virtual DbSet<CustomerEmailAPIRecord> CustomerEmailAPIRecord { get; set; }
        public virtual DbSet<MarketingLeadCallDetail> MarketingLeadCallDetail { get; set; }

        public virtual DbSet<PartialPaymentEmailApiRecord> PartialCustomerEmailAPI { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobStatus> JobStatus { get; set; }
        public virtual DbSet<JobEstimate> JobAssignee { get; set; }
        public virtual DbSet<JobCustomer> JobCustomer { get; set; }
        public virtual DbSet<JobResource> JobResource { get; set; }
        public virtual DbSet<JobNote> JobNote { get; set; }

        public virtual DbSet<AnnualSalesDataUpload> AnnualSalesDataUpload { get; set; }
        public virtual DbSet<AuditInvoice> AuditInvoice { get; set; }
        public virtual DbSet<AuditPayment> AuditPayment { get; set; }
        public virtual DbSet<AuditPaymentItem> AuditPaymentItem { get; set; }
        public virtual DbSet<AuditInvoiceItem> AuditInvoiceItem { get; set; }
        public virtual DbSet<AuditInvoicePayment> AuditInvoicePayment { get; set; }
        public virtual DbSet<SystemAuditRecord> SystemAuditRecord { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<CallDetailData> CallDetailData { get; set; }
        public virtual DbSet<WebLeadData> WebLeadData { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<CustomerSchedulerReminderAudit> CustomerSchdulerReminderAudit { get; set; }
        public virtual DbSet<TechAndSalesSchedulerReminder> TechAndSalesSchedulerReminder { get; set; }
        public virtual DbSet<OrganizationRoleUserFranchisee> OrganizationRoleUserFranchisee { get; set; }
        public virtual DbSet<County> County { get; set; }
        public virtual DbSet<ZipCode> Zipcode { get; set; }
        public virtual DbSet<GeoCodefileupload> GeoCodefileupload { get; set; }

        public virtual DbSet<AuditAddressDiscrepancy> AuditAddressDiscrepency { get; set; }

        public virtual DbSet<AddressHistryLog> AddressHistryLog { get; set; }

        public virtual DbSet<AuditAddress> AuditAddress { get; set; }

        public virtual DbSet<AuditCustomer> AuditCustomer { get; set; }

        public virtual DbSet<AuditFranchiseeSales> AuditFranchiseeSales { get; set; }


        public virtual DbSet<AnnualReportType> AnnualReportType { get; set; }
        public virtual DbSet<InvoiceAddress> InvoiceAddress { get; set; }
        public virtual DbSet<AnnualRoyality> AnnualRoyality { get; set; }
        public virtual DbSet<JobEstimateServices> JobEstimateServices { get; set; }
        public virtual DbSet<JobEstimateImageCategory> JobEstimateImageCategory { get; set; }

        public virtual DbSet<JobEstimateImage> jobEstimateImage { get; set; }
        public virtual DbSet<BeforeAfterImageMailAudit> BeforeAfterImageMailAudit { get; set; }
        public virtual DbSet<LoanAdjustmentAudit> LoanadjustmentAudit { get; set; }
        public virtual DbSet<MasterMarketingClass> MasterMarketingClass { get; set; }
        public virtual DbSet<SubClassMarketingClass> SubClassMarketingClass { get; set; }

        public virtual DbSet<FranchiseeTechMailService> FranchiseeTechMailService { get; set; }

        public virtual DbSet<FranchiseeFeeEmailInvoiceItem> FranchiseeFeeEmailInvoiceItem { get; set; }

        public virtual DbSet<EquipmentUserDetails> EquipmentUserDetails { get; set; }
        public virtual DbSet<JobDetails> JobDetails { get; set; }

        public virtual DbSet<LeadPerformanceFranchiseeDetails> LeadPerformanceFranchiseeDetails { get; set; }
        public virtual DbSet<ReviewPushAPILocation> ReviewPushAPILocaltion { get; set; }
        public virtual DbSet<ReviewPushCustomerFeedback> ReviewPushCustomerFeedback { get; set; }

        public virtual DbSet<MarkbeforeAfterImagesHistry> MarkbeforeAfterImagesHistry { get; set; }

        public virtual DbSet<ReviewMarketingImageLastDateHistry> ReviewMarketingImageLastDateHistry { get; set; }

        public virtual DbSet<OnetimeprojectfeeAddFundRoyality> OnetimeprojectfeeAddFundRoyality { get; set; }
        public virtual DbSet<FranchiseeRegistrationHistry> FranchiseeRegistrationHistry { get; set; }
        public virtual DbSet<HomeAdvisor> HomeAdvisor { get; set; }
        public virtual DbSet<ToDoFollowUpList> ToDoFollowUpList { get; set; }
        public virtual DbSet<ToDoFollowUpComment> ToDoFollowUpComment { get; set; }
        public virtual DbSet<Perpetuitydatehistry> Perpetuitydatehistry { get; set; }

        public virtual DbSet<FranchiseeDurationNotesHistry> FranchiseeDurationNotesHistry { get; set; }

        public virtual DbSet<BeforeAfterImages> BeforeAfterImages { get; set; }
        public virtual DbSet<MarketingLeadCallDetailV2> MarketingLeadCallDetailV2 { get; set; }
        public virtual DbSet<EstimateInvoiceCustomer> EstimateInvoiceCustomer { get; set; }
        public virtual DbSet<EstimateInvoiceService> EstimateInvoiceService { get; set; }
        public virtual DbSet<EstimateInvoice> EstimateInvoice { get; set; }
        public virtual DbSet<UpdateMarketingClassfileupload> UpdateMarketingClassfileupload { get; set; }

        public virtual DbSet<TermsAndConditionFranchisee> TermsAndConditionFranchisee { get; set; }

        public virtual DbSet<EstimateServiceInvoiceNotes> EstimateServiceInvoiceNotes { get; set; }
        public virtual DbSet<CustomerSignature> CustomerSignature { get; set; }
        public virtual DbSet<CustomerSignatureInfo> CustomerSignatureInfo { get; set; }
        public virtual DbSet<CustomerLog> CustomerLog { get; set; }

        public virtual DbSet<DebuggerLogs> DebuggerLogs { get; set; }

        public virtual DbSet<TechnicianWorkOrderForInvoice> TechnicianWorkOrderForInvoice { get; set; }
        public virtual DbSet<TechnicianWorkOrder> TechnicianWorkOrder { get; set; }

        public virtual DbSet<FranchiseeTechMailEmail> FranchiseeTechMailEmail { get; set; }
        public virtual DbSet<EstimateInvoiceDimension> EstimateInvoiceDimension { get; set; }
        public virtual DbSet<EstimateInvoiceAssignee> EstimateInvoiceAssignee { get; set; }
        public virtual DbSet<MarketingLeadCallDetailV3> MarketingLeadCallDetailV3 { get; set; }
        public virtual DbSet<MarketingLeadCallDetailV4> MarketingLeadCallDetailV4 { get; set; }
        public virtual DbSet<MarketingLeadCallDetailV5> MarketingLeadCallDetailV5 { get; set; }

        public virtual DbSet<TermsAndCondition> TermsAndCondition { get; set; }
        public virtual DbSet<ServicesTag> ServicesTag { get; set; }
        public virtual DbSet<PriceEstimateServices> PriceEstimateServices { get; set; }
        public virtual DbSet<TaxRates> TaxRates { get; set; }
        public virtual DbSet<SalesTaxRates> SalesTaxRates { get; set; }
        public virtual DbSet<ShiftCharges> ShiftCharges { get; set; }
        public virtual DbSet<ReplacementCharges> ReplacementCharges { get; set; }
        public virtual DbSet<HoningMeasurement> HoningMeasurement { get; set; }
        public virtual DbSet<EstimatePriceNotes> EstimatePriceNotes { get; set; }
        public virtual DbSet<EstimateInvoiceServiceImage> EstimateInvoiceServiceImage { get; set; }
        public virtual DbSet<CallDetailsReportNotes> CallDetailsReportNotes { get; set; }
        public virtual DbSet<PriceEstimateFileUpload> PriceEstimateFileUpload { get; set; }
        public virtual DbSet<EmailSignatures> EmailSignatures { get; set; }
        public virtual DbSet<HoningMeasurementDefault> HoningMeasurementDefault { get; set; }
        public virtual DbSet<FranchsieeGoogleReviewUrlAPI> FranchsieeGoogleReviewUrlAPI { get; set; }
        public virtual DbSet<CustomerJobEstimate> CustomerJobEstimate { get; set; }
    }


    public class DbConnection
    {
        public const string DbContextConnectionAttribute = "ConnectionString";
    }
}
