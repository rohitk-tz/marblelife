Alter table servicestag
add column IsActive bit(1) default b'1';

update servicestag
set MaterialType = null where MaterialType = 'Marble' And CategoryId = 288;

update servicestag
Set IsActive = b'0' where CategoryId = 288 and MaterialType Is Not null;

update servicestag
set MaterialType = null where MaterialType = 'Marble' And CategoryId = 287;

update servicestag
Set IsActive = b'0' where CategoryId = 287 and MaterialType Is Not null;

update servicestag
Set IsActive = b'0' where CategoryId = 286;

Update servicestag 
Set materialtype = 'Metal' Where ServiceTypeId = 15 and MaterialType = 'Marble';

update servicestag
Set IsActive = b'0' where ServiceTypeId = 15 and MaterialType != 'Metal';

Update servicestag 
Set materialtype = 'Wood' Where ServiceTypeId = 40 and MaterialType = 'Marble';

update servicestag
Set IsActive = b'0' where ServiceTypeId = 40 and MaterialType != 'Wood';

Update servicestag 
Set materialtype = 'Carpet' Where ServiceTypeId = 16 and MaterialType = 'Marble';

update servicestag
Set IsActive = b'0' where ServiceTypeId = 16 and MaterialType != 'Carpet';

update servicestag
Set IsActive = b'0' where CategoryId = 285;

Update servicestag 
Set materialtype = 'Glass' Where ServiceTypeId = 13 and MaterialType = 'Marble';

update servicestag
Set IsActive = b'0' where ServiceTypeId = 13 and MaterialType != 'Glass';

update servicestag
Set IsActive = b'0' where ServiceTypeId = 11 and categoryId = 282 and MaterialType In ('Ceramic', 'Porcelain', 'Corian', 'Vinyl');

update servicestag
Set IsActive = b'0' where ServiceTypeId = 34 and categoryId = 282 and Service = 'Marblized Top Coat Installation' and MaterialType In ('Vinyl');

update servicestag
Set IsActive = b'0' where ServiceTypeId = 5 and categoryId = 282 and Service = 'Sealing' and MaterialType In ('Granite');

update servicestag
Set IsActive = b'0' where ServiceTypeId = 5 and categoryId = 282 and Service In ('Engineered Stone - Polish', 'Engineered Stone - Scratch Removal');

update servicestag
Set IsActive = b'0' where ServiceTypeId = 3 and categoryId = 282 and Service In ('Colorseal-Powerwash', 'Cleaning', 'Colorseal-Hydroforce') and MaterialType = 'Corian'; 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 3 and categoryId = 282 and Service In ('Colorseal-Powerwash', 'Colorseal-Hydroforce') and MaterialType = 'Vinyl'; 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 17 and categoryId = 282 and Service In ('Grout Sealing - Clear') and MaterialType = 'Vinyl'; 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 4 and categoryId = 282 and Service In ('VinylGuard Installation') and MaterialType != 'Vinyl'; 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 4 and categoryId = 282 and Service In ('Cleaning and Pre-Coat Preparations') and MaterialType Not in ('Vinyl', 'Terrazzo'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 284 and Service In ('Endurachip - Fast' , 'Endurachip - Light') and MaterialType Not in ('Concrete', 'Terrazzo', 'Slate'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 3 and categoryId = 284 and Service In ('Tile Replacement') and MaterialType In ('Corian'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 17 and categoryId = 284 and Service In ('Tile Removal') and MaterialType In ('Corian' , 'Vinyl'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 3 and categoryId = 283 and Service In ('Grout Replacement') and MaterialType In ('Corian' , 'Vinyl'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 3 and categoryId = 283 and Service In ('Grout Repair') and MaterialType In ('Vinyl'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Anti-Spauling Driveway Sealing') and MaterialType not In ('Concrete', 'Slate'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Anti-Spauling Sideway Sealing') and MaterialType not In ('Concrete', 'Slate'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Cleaning') and MaterialType not In ('Concrete', 'Terrazzo','Slate');

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Endurachip - Durable') and MaterialType In ('Ceramic'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('MARBLIZED COATING') and MaterialType not In ('Concrete', 'Terrazzo','Slate'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Moisture Vapor Barrier', 'Polyspartic Base', 'Polyspartic Top Coat') and MaterialType not In ('Concrete', 'Terrazzo','Slate'); 

update servicestag
Set IsActive = b'0' where ServiceTypeId = 33 and categoryId = 282 and Service In ('Vinyl Chips') and MaterialType not In ('Concrete', 'Slate'); 

update servicestag
Set IsActive = b'1' where ServiceTypeId = 35 and categoryId = 285 and Service In ('Concrete Preparations') and MaterialType In ('Concrete','Terrazzo'); 

update servicestag
Set CategoryId = 282 where ServiceTypeId = 35 and categoryId = 285 and Service In ('Concrete Preparations') and MaterialType In ('Concrete','Terrazzo'); 



UPDATE `makalu`.`servicetype` SET `ColorCode` = '#FFAF33' WHERE (`Id` = '42');


Insert Into lookup (Id, LookupTypeId, Name, Alias, RelativeOrder, IsActive, IsDeleted) Values(299, 46, '', 'EVENT', 1, b'1', b'0');