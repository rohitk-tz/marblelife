ALTER TABLE `person` 
ADD COLUMN `isRecruitmentFeeApplicable` BIT(1) NOT NULL DEFAULT b'0' AFTER `IsDeleted`;
