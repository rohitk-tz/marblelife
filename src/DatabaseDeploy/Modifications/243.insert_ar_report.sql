UPDATE `emailtemplate` SET `Body`='<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n   <html xmlns=\"http://www.w3.org/1999/xhtml\">\n   <head>\n       <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n       <title>@Model.Base.ApplicationName</title>\n       <style type=\"text/css\">\n           body {\n               padding-left: 10px !important;\n               padding-right: 10px !important;\n               padding-top: 10px !important;\n               padding-bottom: 10px !important;\n               margin: 20px !important;\n               width: 600px;\n               border: solid 1px #B2C8D9;\n               -webkit-text-size-adjust: 100% !important;\n               -ms-text-size-adjust: 100% !important;\n               -webkit-font-smoothing: antialiased !important;\n               font-size: 16px;\n           }\n           a {\n               color: #382F2E;\n           }\n   \n           p, h1 {\n               margin: 0;\n           }\n   \n           p {\n               text-align: left;\n               font-weight: normal;\n               line-height: 19px;\n           }\n   \n           a.link1 {\n               color: #382F2E;\n           }\n   \n           a.link2 {\n               text-decoration: none;\n               color: #ffffff;\n           }\n   \n           h2 {\n               text-align: left;\n               color: #222222;\n               font-size: 19px;\n               font-weight: normal;\n           }\n   \n           div, p, ul, h1 {\n               margin: 0;\n           }\n   \n           .bgBody {\n               background: #ffffff;\n           }\n   \n           table thead tr th {\n               border: 1px solid black;\n               padding: 5px;\n               margin: 2px;\n               text-align: left;\n           }\n   \n           table tbody tr td {\n               border: 1px solid black;\n               padding: 5px;\n               margin: 2px;\n               text-align: left;\n           }\n       </style>\n   </head>\n   <body style=\"background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\">\n           <div class=\"contentEditableContainer contentTextEditable\">\n               <div class=\"contentEditable\">\n                   <span>Dear  @Model.Base.OwnerName,</span>\n                   <br>\n                   <br />\n                   <p>\n 				  @if(Model.WeeklyCollection.Count() <= 0)\n 					{\n 					  <span>No unpaid Invoices for any Franchisee during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>\n 					  <br /><br />\n 					}\n  				@if(Model.WeeklyCollection.Count() > 0)\n 				{\n   						<span>Following is the list of unpaid invoices generated during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>\n                       <br />\n                       <br />\n                       <table style=\"border: 1px solid black;border-collapse:collapse;\">\n                           <thead>\n                               <tr>\n                                   <th style=\"border: 1px solid black;\">Invoice#</th>\n                                   <th style=\"border: 1px solid black;\">Franchisee</th>\n                                   <th style=\"border: 1px solid black;\">Start Date</th>\n                                   <th style=\"border: 1px solid black;\">End Date</th>\n                                   <th style=\"border: 1px solid black;\">Due Date</th>\n 								  <th style=\"border: 1px solid black;\">Invoice Amount</th>\n 								  <th style=\"border: 1px solid black;\">Late Fee Applicable</th>\n 								  <th style=\"border: 1px solid black;\">Payable Amount</th>\n                               </tr>\n                           </thead>\n   						 @foreach(var item in Model.WeeklyCollection) \n 						 { \n                           <tbody>\n                               <tr>\n                                   <td style=\"border: 1px solid black;\">@item.InvoiceId</td>\n                                   <td style=\"border: 1px solid black;\">@item.Franchisee</td>\n                                   <td style=\"border: 1px solid black;\">@item.StartDate</td>\n                                   <td style=\"border: 1px solid black;\">@item.EndDate</td>\n                                   <td style=\"border: 1px solid black;\">@item.DueDate</td>\n 								  <td style=\"border: 1px solid black;\">$@item.InvoiceAmount</td>\n                                   <td style=\"border: 1px solid black;\">@item.LateFeeApplicable</td>\n 								  <td style=\"border: 1px solid black;\">$@item.PayableAmount</td>\n                              </tr>\n                           </tbody>\n   						}\n                       </table>  \n                       <br />\n                       <br />  \n 				} \n 				 @if(Model.PreviousCollection.Count() <= 0)\n 					{\n 					  <span>No unpaid Invoices for any Franchisee before <b>@Model.StartDate</b>.</span>\n 					  <br /><br />\n 					}\n 				@if(Model.PreviousCollection.Count() > 0)\n 				{\n   						<span>Following is the list of unpaid invoices generated Before <b>@Model.StartDate</b>.</span>\n                       <br />\n                       <br />\n                       <table style=\"border: 1px solid black;border-collapse:collapse;\">\n                           <thead>\n                               <tr>\n                                   <th style=\"border: 1px solid black;\">Invoice#</th>\n 								  <th style=\"border: 1px solid black;\">Franchisee</th>\n                                   <th style=\"border: 1px solid black;\">Start Date</th>\n                                   <th style=\"border: 1px solid black;\">End Date</th>\n                                   <th style=\"border: 1px solid black;\">Due Date</th>\n 								  <th style=\"border: 1px solid black;\">Invoice Amount</th>\n 								  <th style=\"border: 1px solid black;\">Late Fee Applicable</th>\n                                   <th style=\"border: 1px solid black;\">Payable Amount</th>\n                               </tr>\n                           </thead>\n   						 @foreach(var item in Model.PreviousCollection) \n 						 { \n                           <tbody>\n                               <tr>\n                                   <td style=\"border: 1px solid black;\">@item.InvoiceId</td>\n                                   <td style=\"border: 1px solid black;\">@item.Franchisee</td>\n                                   <td style=\"border: 1px solid black;\">@item.StartDate</td>\n                                   <td style=\"border: 1px solid black;\">@item.EndDate</td>\n                                   <td style=\"border: 1px solid black;\">@item.DueDate</td>\n 								  <td style=\"border: 1px solid black;\">$@item.InvoiceAmount</td>\n                                   <td style=\"border: 1px solid black;\">@item.LateFeeApplicable</td>\n                                   <td style=\"border: 1px solid black;\">$@item.PayableAmount</td>\n                              </tr>\n                           </tbody>\n   						}\n                       </table>  \n                       <br />\n                       <br />  \n 					}\n                       Regards,\n                       <br /><br />\n   					<p>                    \n   					@Model.Base.CompanyName <br>\n   					</p>\n                   </p>\n               </div>\n           </div>\n   </body>\n   </html>' WHERE `Id`='10';


INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) VALUES ('33', 'AR Report', 'AR Report', b'1', b'1', b'0');

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) VALUES ('33', '33', 'AR Report', 'AR Report', 'AR Report Notification', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
          <html xmlns="http://www.w3.org/1999/xhtml">
          <head>
              <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
              <title>@Model.Base.ApplicationName</title>
              <style type="text/css">
                  body {
                      padding-left: 10px !important;
                      padding-right: 10px !important;
                      padding-top: 10px !important;
                      padding-bottom: 10px !important;
                      margin: 20px !important;
                      width: 600px;
                      border: solid 1px #B2C8D9;
                      -webkit-text-size-adjust: 100% !important;
                      -ms-text-size-adjust: 100% !important;
                      -webkit-font-smoothing: antialiased !important;
                      font-size: 16px;
                  }
                  a {
                      color: #382F2E;
                  }
          
                  p, h1 {
                      margin: 0;
                  }
          
                  p {
                      text-align: left;
                      font-weight: normal;
                      line-height: 19px;
                  }
          
                  a.link1 {
                      color: #382F2E;
                  }
          
                  a.link2 {
                      text-decoration: none;
                      color: #ffffff;
                  }
          
                  h2 {
                      text-align: left;
                      color: #222222;
                      font-size: 19px;
                      font-weight: normal;
                  }
          
                  div, p, ul, h1 {
                      margin: 0;
                  }
          
                  .bgBody {
                      background: #ffffff;
                  }
          
                  table thead tr th {
                      border: 1px solid black;
                      padding: 5px;
                      margin: 2px;
                      text-align: left;
                  }
          
                  table tbody tr td {
                      border: 1px solid black;
                      padding: 5px;
                      margin: 2px;
                      text-align: left;
                  }
              </style>
          </head>
          <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                  <div class="contentEditableContainer contentTextEditable">
                      <div class="contentEditable">
                          <span>Dear @Model.Base.OwnerName,</span>
                          <br>
                          <br />
                          <p>
         				
          				@if(Model.WeeklyCollectionFranchiseeWise.Count() > 0)
         				{
         					  <span>Following is the list of unpaid invoices generated Franchisee wise till  <b>@Model.EndDate</b>.</span>
         						<br /><br />
                              <table style="border: 1px solid black;border-collapse:collapse;">
                                  <thead>
                                      <tr>
                                          <th style="border: 1px solid black;">Franchisee</th>
                                          <th style="border: 1px solid black;">1-30 Days</th>
         								    <th style="border: 1px solid black;">31-60 Days</th>
         								    <th style="border: 1px solid black;">61-90 Days</th>
                                          <th style="border: 1px solid black;">90+ days</th>
                                          <th style="border: 1px solid black;">TOTAL</th>
                                      </tr>
                                  </thead>
          						 @foreach(var item in Model.WeeklyCollectionFranchiseeWise) 
         						 { 
                                  <tbody>
                                      <tr>
                                          <td style="border: 1px solid black;">@item.Franchisee</td>
                                          <td style="border: 1px solid black;">@item.Thirty</td>
         							    	 <td style="border: 1px solid black;">@item.Sixty</td>
                                          <td style="border: 1px solid black;">@item.Ninety</td>
                                          <td style="border: 1px solid black;">@item.moreThanNinety</td>
                                          <td style="border: 1px solid black;">@item.Total</td>
                                      </tr>
                                  </tbody>
          						}
                              </table> 
                              <br />
                              <br /> 
         				}
         				
                              Regards,
                              <br /><br />
          					<p>                    
          					@Model.Base.CompanyName <br>
          					</p>
                          </p>
                      </div>
                  </div>
          </body>
          </html>');
