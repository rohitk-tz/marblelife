UPDATE `emailtemplate` SET `Body`='<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n          <html xmlns=\"http://www.w3.org/1999/xhtml\">\n          <head>\n              <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n              <title>@Model.Base.ApplicationName</title>\n              <style type=\"text/css\">\n                  body {\n                      padding-left: 10px !important;\n                      padding-right: 10px !important;\n                      padding-top: 10px !important;\n                      padding-bottom: 10px !important;\n                      margin: 20px !important;\n                      width: 600px;\n                      border: solid 1px #B2C8D9;\n                      -webkit-text-size-adjust: 100% !important;\n                      -ms-text-size-adjust: 100% !important;\n                      -webkit-font-smoothing: antialiased !important;\n                      font-size: 16px;\n                  }\n          \n                  a {\n                      color: #382F2E;\n                  }\n          \n                  p, h1 {\n                      margin: 0;\n                  }\n          \n                  p {\n                      text-align: left;\n                      font-weight: normal;\n                      line-height: 19px;\n                  }\n          \n                  a.link1 {\n                      color: #382F2E;\n                  }\n          \n                  a.link2 {\n                      text-decoration: none;\n                      color: #ffffff;\n                  }\n          \n                  h2 {\n                      text-align: left;\n                      color: #222222;\n                      font-size: 19px;\n                      font-weight: normal;\n                  }\n          \n                  div, p, ul, h1 {\n                      margin: 0;\n                  }\n          \n                  .bgBody {\n                      background: #ffffff;\n                  }\n          \n                  table thead tr th {\n                      border: 1px solid black;\n                      padding: 5px;\n                      margin: 2px;\n                      text-align: left;\n                  }\n          \n                  table tbody tr td {\n                      border: 1px solid black;\n                      padding: 5px;\n                      margin: 2px;\n                      text-align: left;\n                  }\n              </style>\n          </head>\n          <body style=\"background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\">\n              <div class=\"contentEditableContainer contentTextEditable\">\n                  <div class=\"contentEditable\">\n                      <span>Dear @Model.FullName,</span>\n                      <br />\n                      <br />\n                      <p>\n                      We have a LAST MINTUE @Model.jobTypeName that has been added to your schedule for @Model.Time with @Model.TechName at @Model.Address. The Client number is @Model.CustomerPhoneNumber.\n                      <br />\n                      Good Luck!\n                      </p>\n                       <br/>\n                      Regards,\n                      <br/>\n                  </div>\n                  <p>\n                             MARBLELIFE <sup>&reg;</sup> <br />\n                             @Model.AssigneePhone <br />\n                  </p>\n          \n              </div>\n          \n          </body>\n          </html>' WHERE `Id`='30';


UPDATE `emailtemplate` SET `Body`='<table border="0" width="600" cellpadding="20" cellspacing="0" class="contenttable">
		 <tbody>
		 <tr>
		 <td bgcolor="#ffffff" style="border:2px solid #f2f2f2;border-collapse:collapse;">
		 <table width="540" border="0" cellspacing="0" cellpadding="0" align="left" class="contenthalf">
		 <tbody>
		 <tr>
		 <td>
			   <font size="3" color="#454545">
					 Dear Sir/Ma''am,<br /><br />
						Thank you for visiting us at @Model.Franchisee. We appreciate your business and value you as a customer. 
						To help us continue our high quality of service, we invite you to leave us your feedback.<br><br>
						<a href=@Model.Link
						style="text-decoration:none;background-color:#f47322;text-transform:uppercase;white-space:nowrap;width:320px;color:#ffffff;font-size:16px;font-weight:bold;padding:6px 0;text-align:center;" rel="nofollow">&nbsp;GIVE FEEDBACK&nbsp;</a><br><br>
						We look forward to seeing you again soon.<br><br>
						Sincerely,<br><br>
						@Model.Owner<br>
						MARBLELIFE <sup>&reg;</sup><br><a href=""><font color="#454545"></font></a>
				</font>
		 </td>
		 </tr>
		 </tbody>
		 </table>
		 </td>
		 </tr>
		 </tbody>
		 </table>' WHERE `Id`='11';