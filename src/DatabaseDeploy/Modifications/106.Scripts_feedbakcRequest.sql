ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `IsSystemGenerated` BIT(1) NOT NULL DEFAULT b'0' AFTER `QBInvoiceId`;

ALTER TABLE `customerfeedbackrequest` 
CHANGE COLUMN `CustomerReviewSystemRecordId` `CustomerReviewSystemRecordId` BIGINT(20) NULL ;

INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('11', 'Customer Feedback Request', 'Customer Feedback Request', 1, 1, 0);

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('11', '11', 'Feedback', 'Feedback', 'Thank you for visiting us. Would you leave us your feedback?', '<table border="0" width="600" cellpadding="20" cellspacing="0" class="contenttable">
		 <tbody>
		 <tr>
		 <td bgcolor="#ffffff" style="border:2px solid #f2f2f2;border-collapse:collapse;">
		 <table width="320" border="0" cellspacing="0" cellpadding="0" align="left" class="contenthalf">
		 <tbody>
		 <tr>
		 <td>
			   <font size="3" color="#454545" class="e2w-email-message">
					 Dear Sir/Ma''am,<br>
						Thank you for visiting us at @Model.Franchisee. We appreciate your business and value you as a customer. 
						To help us continue our high quality of service, we invite you to leave us your feedback.<br><br>
						<a href=@Model.Link
						style="text-decoration:none;background-color:#f47322;text-transform:uppercase;white-space:nowrap;width:320px;color:#ffffff;font-size:16px;font-weight:bold;padding:6px 0;text-align:center;" rel="nofollow">&nbsp;GIVE FEEDBACK&nbsp;</a><br><br>
						We look forward to seeing you again soon.<br><br>
						Sincerely,<br><br>
						@Model.Owner<br>
						@Model.Franchisee<br><a href=""><font color="#454545"></font></a>
				</font>
		 </td>
		 </tr>
		 </tbody>
		 </table>
		 </td>
		 </tr>
		 </tbody>
		 </table>');

