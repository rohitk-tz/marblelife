ALTER TABLE `PaymentInstrument` 
DROP FOREIGN KEY `fk_PaymentInstrument_lookup1`;
ALTER TABLE `PaymentInstrument` 
ADD CONSTRAINT `fk_PaymentInstrument_InstrumentType`
  FOREIGN KEY (`InstrumentTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
