ALTER TABLE makalu.beforeafterimagemailaudit DROP FOREIGN KEY fk_beforeAfterImageMailAudit_franchisee;


ALTER TABLE `beforeafterimagemailaudit`  
ADD CONSTRAINT `fk_beforeAfterImageMailAudit_franchisee` 
    FOREIGN KEY (`franchiseeId`) REFERENCES `organization` (`id`) ON DELETE CASCADE;  