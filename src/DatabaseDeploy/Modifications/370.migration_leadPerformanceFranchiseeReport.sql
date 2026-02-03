USE `makalu`;
DROP procedure IF EXISTS `migration_lead_performance_franchisee_deteatils`;

DELIMITER $$
USE `makalu`$$
CREATE PROCEDURE `migration_lead_performance_franchisee_deteatils` ()
BEGIN

declare _index int;
declare _id int;

create table distinctIds
select distinct Id from  `makalu`.`franchisee`;
 
 
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 select id into _index from distinctIds Limit 1;

 
INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2019-07-01 16:07:59',7, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES ( _index, 220, 275, '2019-07-01 16:07:59',7, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2019-08-01 16:07:59',8, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2019-08-01 16:07:59',8, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES ( _index, 219, 50, '2019-09-01 16:07:59',9, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2019-09-01 16:07:59',9, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2019-10-01 16:07:59',10, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES ( _index, 220, 275, '2019-10-01 16:07:59',10, b'1', b'0',17039981);



INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2019-11-01 16:07:59',11, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2019-11-01 16:07:59',11, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2019-12-01 16:07:59',12, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2019-12-01 16:07:59',12, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-01-01 16:07:59',1, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-01-01 16:07:59',1, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-02-01 16:07:59',2, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-02-01 16:07:59',2, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-03-01 16:07:59',3, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-03-01 16:07:59',3, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-04-01 16:07:59',4, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-04-01 16:07:59',4, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-05-01 16:07:59',5, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-05-01 16:07:59',5, b'1', b'0',17039981);


INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-06-01 16:07:59',6, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES ( _index, 220, 275, '2020-06-01 16:07:59',6, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-07-01 16:07:59',7, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-07-01 16:07:59',7, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES ( _index, 219, 50, '2020-08-01 16:07:59',8, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` ( `franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-08-01 16:07:59',8, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 219, 50, '2020-09-01 16:07:59',9, b'1', b'0',17039981);

INSERT INTO `makalu`.`leadperformancefranchiseedetails` (`franchiseeId`, `categoryId`, `Amount`, `DateTime`, `Month`, `IsActive`
, `IsDeleted`,`DataRecorderMetaDataId`) 
VALUES (_index, 220, 275, '2020-09-01 16:07:59',9, b'1', b'0',17039981);

DELETE FROM distinctIds WHERE  id = _index;
	   
 END WHILE;
  
  DROP TABLE distinctIds;
END$$

DELIMITER ;

