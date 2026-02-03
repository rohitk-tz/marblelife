SET SQL_SAFE_UPDATES = 0;

update batchuploadrecord set  isdeleted = 1  where franchiseeid = 62;

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 32, 10,'2017-03-01','2017-03-31','2017-04-11',1,'2017-04-11');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 32, 10,'2017-04-01','2017-04-30','2017-05-10',0,'2017-05-17');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 32, 10,'2017-05-01','2017-05-31','2017-06-11',0,'2017-06-16');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 32, 10,'2017-06-01','2017-06-30','2017-07-10',1,'2017-07-09');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 32, 10,'2017-07-01','2017-07-31','2017-08-11',0,'2017-08-17');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-07-31','2017-08-06','2017-08-09',0,'2017-09-07');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-08-07','2017-08-13','2017-08-16',0,'2017-09-07');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-08-14','2017-08-20','2017-08-23',0,'2017-09-11');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-08-21','2017-08-27','2017-08-30',0,'2017-09-11');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-08-28','2017-09-03','2017-09-06',0,'2017-09-11');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-09-04','2017-09-10','2017-09-13',1,'2017-09-11');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-09-11','2017-09-17','2017-09-20',0,'2017-10-04');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-09-18','2017-09-24','2017-09-27',0,'2017-10-04');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-09-25','2017-10-04','2017-09-27',1,'2017-10-04');

INSERT INTO `batchuploadrecord` (`FranchiseeId`, `PaymentFrequencyId`, `waitPeriod`, `startdate`, `endDate`, `expectedUploadDate`,`iscorrectUploaded`,`uploadedon`) 
VALUES (62, 31, 3,'2017-10-02','2017-10-08','2017-10-11',1,'2017-10-10');


SET SQL_SAFE_UPDATES = 0;

