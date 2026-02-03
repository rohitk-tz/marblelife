SET SQL_SAFE_UPDATES=0;
UPDATE franchisee AS U1, organization AS U2 
SET U1.ContactEmail = U2.Email
WHERE U1.Id = U2.Id;
SET SQL_SAFE_UPDATES=1;



SET SQL_SAFE_UPDATES=0;
UPDATE franchisee AS U1, franchisee AS U2 
SET U1.ContactFirstName = (SELECT SUBSTRING_INDEX(U2.OwnerName, ' ', 1)),
U1.ContactLastName=(SELECT SUBSTRING_INDEX(SUBSTRING_INDEX(U2.OwnerName,' ',2),' ',-1))
WHERE U1.Id = U2.Id;
SET SQL_SAFE_UPDATES=1;
