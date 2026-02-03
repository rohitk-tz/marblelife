Create Table `shiftcharges`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`FranchiseeId` BIGINT(20) NULL ,
`TechDayShiftPrice` DECIMAL(18,2) Null,
`CommercialRestorationShiftPrice` DECIMAL(18,2) Null,
`MaintenanceTechNightShiftPrice` DECIMAL(18,2) NULL ,
`IsPriceChangedByFranchisee` BIT(1) NULL DEFAULT b'0',
`IsActive` BIT(1) NULL DEFAULT b'0',
`IsDeleted` BIT(1) NULL DEFAULT b'0' ,
`DataRecorderMetaDataId` BigInt(20) null,
PRIMARY KEY (`Id`),
INDEX `fk_shiftcharges_franchiseeId_idx` (`FranchiseeId` ASC) ,
CONSTRAINT `fk_shiftcharges_franchiseeid_idx`
FOREIGN KEY (`FranchiseeId`)
REFERENCES `franchisee` (`Id`),
INDEX `fk_shiftcharges_datarecordermetadataId_idx` (`DataRecorderMetaDataId` ASC) ,
CONSTRAINT `fk_shiftcharges_datarecordermetadataId_idx`
FOREIGN KEY (`DataRecorderMetaDataId`)
REFERENCES `DataRecorderMetaData` (`Id`)
);



insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (1,Null,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (2,3,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (3,4,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (4,5,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (5,6,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (6,8,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (7,9,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (8,10,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (9,11,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (10,14,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (11,15,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (12,16,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (13,17,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (14,18,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (15,19,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (16,20,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (17,21,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (18,22,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (19,23,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (20,25,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (21,27,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (22,29,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (23,30,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (24,31,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (25,38,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (26,39,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (27,40,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (28,41,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (29,42,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (30,45,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (31,46,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (32,47,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (33,48,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (34,50,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (35,51,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (36,52,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (37,53,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (38,54,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (39,56,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (40,57,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (41,61,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (42,62,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (43,64,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (44,70,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (45,72,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (46,74,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (47,75,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (48,76,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (49,77,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (50,78,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (51,79,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (52,80,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (53,81,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (54,82,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (55,83,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (56,84,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (57,85,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (58,86,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (59,87,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (60,88,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (61,90,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (62,91,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (63,92,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (64,93,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (65,94,1400,885,600,b'0',b'1',b'0');
insert into shiftcharges(Id, FranchiseeId, TechDayShiftPrice, CommercialRestorationShiftPrice, MaintenanceTechNightShiftPrice, IsPriceChangedByFranchisee, IsActive, IsDeleted) values (66,95,1400,885,600,b'0',b'1',b'0');

