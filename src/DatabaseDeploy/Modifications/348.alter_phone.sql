ALTER TABLE `phone` 
ADD COLUMN `IsTransferable` bit(1)  default 0;

INSERT INTO `makalu`.`marketingclass` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('31', 'THEATER', '#CD5CC', b'0');