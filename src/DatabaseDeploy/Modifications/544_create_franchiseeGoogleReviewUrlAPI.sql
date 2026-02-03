CREATE TABLE `franchsieeGoogleReviewUrlAPI` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NOT NULL,
  `BusinessName` varchar(200) DEFAULT null,
  `BrightLocalLink` varchar(1022) DEFAULT null,
   `IsDeleted` bit(1) default b'0',
  
  PRIMARY KEY (`ID`),
  KEY `FK_franchsieeGoogleReviewUrlAPI_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `FK_franchsieeGoogleReviewUrlAPI_franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;

INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('47','MARBLELIFE® of Dallas', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/0d883cbfbd5686433221a9c56768388c724fd055');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('78','MARBLELIFE® of San Antonio', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/45412e7c43b6a9c1869d822e61b2c821c5f4bdea');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('48','MARBLELIFE® of Utah', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/8e7991fd4d861033727db58b1cae9bdfa7014787');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('86','MARBLELIFE® of Hampton Roads', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/69d8b14ee3b7c27c7130912f5adac5851988edb0');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('8','MARBLELIFE® of Los Angeles', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('76','MARBLELIFE® of Sacramento', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/edcf325ff9f97009ce5500346837454acdd25056');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('21','MARBLELIFE® of Atlanta', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/db0dda13e167bd1e376786f776c0eebccb600cf0');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('94','MARBLELIFE® of Charlotte', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/eec1fe31138c5a69c223f71949ff30606be20711');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('90','MARBLELIFE® of Houston', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/49b33aaedf8eaa0f7a946c3b9c60cb317a8f9b8c');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('92','MARBLELIFE® of Houston West', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/bac6ecfd69ec7c422fa555872af753da2de9c223');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('10','MARBLELIFE® of Orange County', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/303f57ce3cde77822b64f983dd8ae6b1fb9d3f10');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('14','MARBLELIFE® of Tampa Bay', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/84cb121805eebc38bb048b0c41acde7d6cbd875a');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('3','MARBLELIFE® of Southwest Alabama', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/3294a357bac8427ae784f3a2dee2233fb23ccb54');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('41','MARBLELIFE® of Portland', 'https://www.local-marketing-reports.com/external/showcase-reviews/widgets/3183e57da009e31f60e9b2505c3b9ca165aec03c');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('52','MARBLELIFE® of Vancouver', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('57','MARBLELIFE® of South Africa', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('8','MARBLELIFE® of Los Angeles', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('93','MARBLELIFE® of Chihuahua', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('6','MARBLELIFE® of Los Angeles North', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('57','MARBLELIFE® of Washington DC', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('56', 'MARBLELIFE® of Grand Cayman','');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('88','MARBLELIFE® GULF', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('29','MARBLELIFE® of Minneapolis', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('70','MARBLELIFE® of Twin Cities', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('50','MARBLELIFE® of seattle', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('9','MARBLELIFE® of San Diego', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('20','MARBLELIFE® of Columbus', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('72','MARBLELIFE® of West kentucky', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('81','MARBLELIFE® of Kansas City', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('77','MARBLELIFE® of las vegas', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('4','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('51','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('53','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('54','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('84','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('80','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('83','', '');
INSERT INTO `makalu`.`franchsieeGoogleReviewUrlAPI` (`FranchiseeId`,`BusinessName`, `BrightLocalLink`) VALUES ('46','', '');



