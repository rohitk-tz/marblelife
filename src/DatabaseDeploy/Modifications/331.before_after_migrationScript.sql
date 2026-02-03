USE `makalu`;
DROP procedure IF EXISTS `beforeAfter_Migration`;

DELIMITER $$
USE `makalu`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `beforeAfter_Migration`()
BEGIN

declare _index BIGINT(20);
declare _ServiceIndex BIGINT(20);
declare _ImageIndex BIGINT(20);
declare _id BIGINT(20);
declare _DataRecorderMetaDataId BIGINT(20);
declare _FileId BIGINT(20);
declare _SurfaceColor varchar(512);
declare _FinishMaterial varchar(512);
declare _SurfaceMaterial varchar(512);
declare _SurfaceType varchar(512);
declare _PairId BIGINT(20);
declare _BuildingLocation varchar(512);
declare _ServiceTypeId BIGINT(20);
declare _Description varchar(512);
declare _IsBeforeImage bit;
declare _CompanyName varchar(512);
declare _MaidService varchar(512);
declare _PropertyManager varchar(512);
declare _MaidJanitorial varchar(512);
declare _CategoryId BIGINT(20);
declare _IsDeleted bit;
declare _TypeId BIGINT(20);
declare _ServiceId BIGINT(20);
declare _ServiceAfterId BIGINT(20);
declare _PairIdCheck BIGINT(20);
declare _serviceIdImage BIGINT(20);
declare _imageId BIGINT(20);
declare _DataRecorderMetaDataIdForImage BIGINT(20);

create table distinctIds
SELECT distinct category.Id
FROM jobestimateimagecategory category
INNER JOIN jobestimateservices  service ON category.Id=service.CategoryId
INNER JOIN jobestimateimage  images ON service.Id=images.ServiceId 
group by images.ServiceId  having count(images.ServiceId)>1;
 
 
WHILE Exists(select 1 from distinctIds Limit 1) DO
 
Select Id into _index from distinctIds Limit 1;

Create table distinctCategoryIds
SELECT distinct services.Id
FROM jobestimateservices services
where services.CategoryId=_index;

WHILE Exists(select 1 from distinctCategoryIds Limit 1) DO
select Id into _ServiceIndex from distinctCategoryIds Limit 1;

Create table distinctImagesIds
SELECT distinct images.Id
FROM jobestimateimage images
where images.ServiceId=_ServiceIndex;

Select services.PairId into _PairIdCheck  from jobestimateservices services where Id=_ServiceIndex;

WHILE Exists(select 1 from distinctImagesIds Limit 1) DO

Select Id into _ImageIndex from distinctImagesIds Limit 1;


if(_PairIdCheck is null) then

        Select CategoryId ,SurfaceColor,FinishMaterial, SurfaceMaterial,SurfaceType,DataRecorderMetaDataId,PairId,BuildingLocation,ServiceTypeId,IsDeleted,description
        ,TypeId,IsBeforeImage,CompanyName,MaidService,PropertyManager,MAIDJANITORIAL
		into 
		_CategoryId,_SurfaceColor,_FinishMaterial,_SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_PairId,_BuildingLocation,_ServiceTypeId,_IsDeleted,_Description,
        _TypeId ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		from jobestimateservices
        where Id = _ServiceIndex
        Limit 1;
        
        Select ServiceId,FileId,TypeId,DataRecorderMetaDataId
		into 
		_ServiceId,_fileId,_TypeId,_DataRecorderMetaDataIdForImage
		from jobestimateimage
        where Id = _ImageIndex
        Limit 1;
                
        INSERT INTO `jobestimateservices`
		(`CategoryId`,`SurfaceColor`,`FinishMaterial`,`SurfaceMaterial`,`SurfaceType`,`DataRecorderMetaDataId`,`PairId`,`BuildingLocation`,`ServiceTypeId`,`IsDeleted`,
        `description`,`TypeId`,`IsBeforeImage`,`CompanyName`,`MaidService`,`PropertyManager`,`MAIDJANITORIAL`)
		VALUES
		(
		_CategoryId,_SurfaceColor , _FinishMaterial,  _SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,null,_BuildingLocation,_ServiceTypeId,b'0',_Description,
        '203' ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		);
        set _serviceId=LAST_INSERT_ID();
         
        INSERT INTO `jobestimateimage`
		(`ServiceId`,`FileId`,`TypeId`,`DataRecorderMetaDataId`)
		VALUES
		(
		_serviceId,_fileId , _TypeId,  _DataRecorderMetaDataIdForImage
        );
        

		Select CategoryId ,SurfaceColor,FinishMaterial, SurfaceMaterial,SurfaceType,DataRecorderMetaDataId,PairId,BuildingLocation,ServiceTypeId,IsDeleted,description
        ,TypeId,IsBeforeImage,CompanyName,MaidService,PropertyManager,MAIDJANITORIAL
		into 
		_CategoryId,_SurfaceColor,_FinishMaterial,_SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_PairId,_BuildingLocation,_ServiceTypeId,_IsDeleted,_Description,
        _TypeId ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		from jobestimateservices
        where Id = _ServiceIndex+1
		Limit 1;
        
		INSERT INTO `jobestimateservices`
		(`CategoryId`,`SurfaceColor`,`FinishMaterial`,`SurfaceMaterial`,`SurfaceType`,`DataRecorderMetaDataId`,`PairId`,`BuildingLocation`,`ServiceTypeId`,`IsDeleted`,
        `description`,`TypeId`,`IsBeforeImage`,`CompanyName`,`MaidService`,`PropertyManager`,`MAIDJANITORIAL`)
		VALUES
		(
		_CategoryId,_SurfaceColor , _FinishMaterial,  _SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_serviceId,_BuildingLocation,_ServiceTypeId,b'0',_Description,
        '204' ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		);
        update  `jobestimateservices`  set `IsDeleted`=b'1' where Id=_ServiceIndex;
        
        else
        
         Select CategoryId ,SurfaceColor,FinishMaterial, SurfaceMaterial,SurfaceType,DataRecorderMetaDataId,PairId,BuildingLocation,ServiceTypeId,IsDeleted,description
        ,TypeId,IsBeforeImage,CompanyName,MaidService,PropertyManager,MAIDJANITORIAL
		into 
		_CategoryId,_SurfaceColor,_FinishMaterial,_SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_PairId,_BuildingLocation,_ServiceTypeId,_IsDeleted,_Description,
        _TypeId ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		from jobestimateservices
        where Id = _ServiceIndex-1
        Limit 1;
        
              
		INSERT INTO `jobestimateservices`
		(`CategoryId`,`SurfaceColor`,`FinishMaterial`,`SurfaceMaterial`,`SurfaceType`,`DataRecorderMetaDataId`,`PairId`,`BuildingLocation`,`ServiceTypeId`,`IsDeleted`,
        `description`,`TypeId`,`IsBeforeImage`,`CompanyName`,`MaidService`,`PropertyManager`,`MAIDJANITORIAL`)
		VALUES
		(
		_CategoryId,_SurfaceColor , _FinishMaterial,  _SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,null,_BuildingLocation,null,b'0',_Description,
        '203' ,b'0',_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		);
        set _serviceId=LAST_INSERT_ID();
         
            
		Select CategoryId ,SurfaceColor,FinishMaterial, SurfaceMaterial,SurfaceType,DataRecorderMetaDataId,PairId,BuildingLocation,ServiceTypeId,IsDeleted,description
        ,TypeId,IsBeforeImage,CompanyName,MaidService,PropertyManager,MAIDJANITORIAL
		into 
		_CategoryId,_SurfaceColor,_FinishMaterial,_SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_PairId,_BuildingLocation,_ServiceTypeId,_IsDeleted,_Description,
        _TypeId ,_IsBeforeImage,_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		from jobestimateservices
        where Id = _ServiceIndex
        Limit 1;
        
        INSERT INTO `jobestimateservices`
		(`CategoryId`,`SurfaceColor`,`FinishMaterial`,`SurfaceMaterial`,`SurfaceType`,`DataRecorderMetaDataId`,`PairId`,`BuildingLocation`,`ServiceTypeId`,`IsDeleted`,
        `description`,`TypeId`,`IsBeforeImage`,`CompanyName`,`MaidService`,`PropertyManager`,`MAIDJANITORIAL`)
		VALUES
		(
		_CategoryId,_SurfaceColor , _FinishMaterial,  _SurfaceMaterial,_SurfaceType,_DataRecorderMetaDataId,_serviceId,_BuildingLocation,_ServiceTypeId,b'0',_Description,
        '204' ,b'0',_CompanyName,_MaidService,_PropertyManager,_MaidJanitorial
		);
         
        set _ServiceAfterId=LAST_INSERT_ID();
        
        Select ServiceId,FileId,TypeId,DataRecorderMetaDataId
		into 
		_ServiceId,_fileId,_TypeId,_DataRecorderMetaDataIdForImage
		from jobestimateimage
        where Id = _ImageIndex
        Limit 1;
         
        INSERT INTO `jobestimateimage`
		(`ServiceId`,`FileId`,`TypeId`,`DataRecorderMetaDataId`)
		VALUES
		(
		_ServiceAfterId,_fileId , _TypeId,  _DataRecorderMetaDataIdForImage
        );
        
        update  `jobestimateservices`  set `IsDeleted`=b'1' where Id=_ServiceIndex;
        update  `jobestimateservices`  set `IsDeleted`=b'1' where Id=_ServiceIndex-1;
        
end if;


DELETE FROM distinctImagesIds WHERE  Id = _ImageIndex;
END WHILE;
DROP TABLE distinctImagesIds;

DELETE FROM distinctCategoryIds WHERE  Id   = _ServiceIndex;
END WHILE; 
DROP TABLE distinctCategoryIds;

DELETE FROM distinctIds WHERE  Id   = _index;

END WHILE;
DROP TABLE distinctIds;
END$$

DELIMITER ;

