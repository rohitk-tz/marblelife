
DELIMITER $$
DROP PROCEDURE IF EXISTS DataMigrateCreateRoyalityInvoice$$
CREATE PROCEDURE DataMigrateCreateRoyalityInvoice()
BEGIN

	DECLARE done bool DEFAULT FALSE;

	Declare _InvoiceId, _StatusId,_DataRecorderMetaDataId,_FranchiseeId,_SalesDataUploadId bigint;	
   declare _GeneratedOn,_DueDate datetime;

	DECLARE A_cur CURSOR FOR SELECT i.Id,i.GeneratedOn,i.DueDate,i.StatusId,i.DataRecorderMetaDataId,fi.FranchiseeId,fi.SalesDataUploadId
    FROM invoice i inner join franchiseeinvoice fi on i.Id=fi.InvoiceId 
      where i.IsDeleted=false and fi.IsDeleted=false ;

	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

	Open A_cur;

	invoice_loop: LOOP
		FETCH A_cur INTO _InvoiceId, _GeneratedOn,_DueDate,_StatusId,_DataRecorderMetaDataId,_FranchiseeId,_SalesDataUploadId;

		IF done THEN
		  LEAVE invoice_loop;
		END IF;

		BEGIN			
				DECLARE _newInvoiceId bigint ;

				insert into  invoice(GeneratedOn,DueDate,StatusId,DataRecorderMetaDataId,IsDeleted)
                values(_GeneratedOn,_DueDate,_StatusId,_DataRecorderMetaDataId,0);
				set _newInvoiceId = (select LAST_INSERT_ID());

			  

                update invoiceitem set InvoiceId=_newInvoiceId  where InvoiceId=_InvoiceId and itemtypeid=92;

             

               	insert into  franchiseeinvoice(FranchiseeId,InvoiceId,SalesDataUploadId,IsDeleted)
                values(_FranchiseeId,_newInvoiceId,_SalesDataUploadId,0);

	        

		END;

	  END LOOP invoice_loop;

	  CLOSE A_cur;
END$$
DELIMITER ;


call DataMigrateCreateRoyalityInvoice();