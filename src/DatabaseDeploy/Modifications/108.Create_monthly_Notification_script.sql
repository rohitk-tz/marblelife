CREATE TABLE `notificationresource` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `NotificationEmailId` BIGINT(20) NOT NULL ,
  `resourceId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`ID`)  ,
  INDEX `fk_notificationResource_notificationEmail_idx` (`NotificationEmailId` ASC) ,
  INDEX `fk_notificationResource_file_idx` (`resourceId` ASC)  ,
  CONSTRAINT `fk_notificationResource_notificationEmail`
    FOREIGN KEY (`NotificationEmailId`)
    REFERENCES `notificationemail` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_notificationResource_file`
    FOREIGN KEY (`resourceId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) 
VALUES ('12', 'Monthly Customer Notification', 'Monthly Customer Notification', 1, 1, 0);

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('12', '12', 'Monthly Customer List', 'Monthly Customer List', 'Monthly Customer List', 
'<table border="0" width="600" cellpadding="20" cellspacing="0" class="contenttable">
 		 <tbody>
 		 <tr>
 		 <td bgcolor="#ffffff" style="border:2px solid #f2f2f2;border-collapse:collapse;">
 		 <table width="450" border="0" cellspacing="0" cellpadding="0" align="left" class="contenthalf">
 		 <tbody>
 		 <tr>
 		 <td>
 			   <font size="3" color="#454545">
 					 Dear @Model.FullName,<br /><br />
 						 Please find List of customers served during @Model.StartDate - @Model.EndtDate in attachtent below.<br /><br />
 						Sincerely,<br /><br />
						
 					@Model.Base.CompanyName <br>
 					@Model.Base.Phone <br>
 				</font>
 		 </td>
 		 </tr>
 		 </tbody>
 		 </table>
 		 </td>
 		 </tr>
 		 </tbody>
 		 </table>');

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) 
VALUES ('13', 'Monthly Notification', 'Monthly Notification', 1, 1, 0);

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('13', '13', 'Monthly Notification', 'Monthly Notification', 'Monthly Notification', 
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 <html xmlns="http://www.w3.org/1999/xhtml">
 <head>
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
     <title>@Model.Base.ApplicationName</title>
     <style type="text/css">
         body {
             padding-left: 10px !important;
             padding-right: 10px !important;
             padding-top: 10px !important;
             padding-bottom: 10px !important;
             margin: 20px !important;
             width: 600px;
             border: solid 1px #B2C8D9;
             -webkit-text-size-adjust: 100% !important;
             -ms-text-size-adjust: 100% !important;
             -webkit-font-smoothing: antialiased !important;
             font-size: 16px;
         }
         a {
             color: #382F2E;
         }
 
         p, h1 {
             margin: 0;
         }
 
         p {
             text-align: left;
             font-weight: normal;
             line-height: 19px;
         }
 
         a.link1 {
             color: #382F2E;
         }
 
         a.link2 {
             text-decoration: none;
             color: #ffffff;
         }
 
         h2 {
             text-align: left;
             color: #222222;
             font-size: 19px;
             font-weight: normal;
         }
 
         div, p, ul, h1 {
             margin: 0;
         }
 
         .bgBody {
             background: #ffffff;
         }
 
         table thead tr th {
             border: 1px solid black;
             padding: 5px;
             margin: 2px;
             text-align: left;
         }
 
         table tbody tr td {
             border: 1px solid black;
             padding: 5px;
             margin: 2px;
             text-align: left;
         }
     </style>
 </head>
 <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
         <div class="contentEditableContainer contentTextEditable">
             <div class="contentEditable">
                 <span>Dear @Model.FullName,</span>
                 <br>
                 <br />
                 <p>
 				Following is the list of total Customers and Synced customers with MailChimp API during  @Model.Startdate - @Model.EndDate.
 				<br />
 				Please find the Details below:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Franchisee</th>
                                 <th style="border: 1px solid black;">Total Customers</th>
                                 <th style="border: 1px solid black;">Customers With Email</th>
                                 <th style="border: 1px solid black;">Total Synced Customers</th>
							</tr>
                         </thead>						
                        
 						 @{foreach(var item in Model.ListEmailRecord) { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.Franchisee</td>
                                 <td style="border: 1px solid black;">@item.TotalCustomers</td>
                                 <td style="border: 1px solid black;">@item.CustomersWithEmail</td>
                                 <td style="border: 1px solid black;">@item.SyncedCustomers</td>
                             </tr>
                         </tbody>
 						}}
                     </table>                    
					<br>
                   <br />				   
				   <p>
   				Following is the list of total Franchisee and the details of customers for Review system during  @Model.Startdate - @Model.EndDate.
   				<br />
   				Please find the Details below:
                       <br>
                       <br />
                       <table style="border: 1px solid black;border-collapse:collapse;">
                           <thead>
                               <tr>
                                   <th style="border: 1px solid black;">Franchisee</th>
                                   <th style="border: 1px solid black;">Total Customers</th>
                                   <th style="border: 1px solid black;">Total Request Send</th>
                                   <th style="border: 1px solid black;">Total Response Received</th>
 								  <th style="border: 1px solid black;">Total Kiosk Link Send</th>
  							</tr>
                           </thead>						
                          
   						 @{foreach(var item in Model.ListReview) { 
                           <tbody>
                               <tr>
                                   <td style="border: 1px solid black;">@item.Franchisee</td>
                                   <td style="border: 1px solid black;">@item.TotalCustomers</td>
                                   <td style="border: 1px solid black;">@item.TotalRequests</td>
                                   <td style="border: 1px solid black;">@item.TotalResponse</td>
 								  <td style="border: 1px solid black;">@item.TotalKioskLink</td>
                               </tr>
                           </tbody>
   						}}
                       </table>   		   
				   		   
				   				   
                     <br>
                     <br />
                     Regards,
                     <br /><br />
 					<p>
                    @Model.Base.CompanyName <br>
 					@Model.Base.Phone <br>
 					</p>
                 </p>
             </div>
         </div>
 </body>
 </html>'
);

