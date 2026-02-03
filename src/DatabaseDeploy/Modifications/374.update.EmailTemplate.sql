UPDATE `makalu`.`emailtemplate` SET `Body` = '\n  \n     \n   \n      <!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n                         <html xmlns=\"http://www.w3.org/1999/xhtml\"> <head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>@Model.Base.CompanyName</title><style type=\"text/css\">body {\n                                     padding-left: 10px !important;\n                                     padding-right: 10px !important;\n                                     padding-top: 10px !important;\n                                     padding-bottom: 10px !important;\n                                     margin: 20px !important;\n                                     width: 600px;\n                                     border: solid 1px #B2C8D9;\n                                     -webkit-text-size-adjust: 100% !important;\n                                     -ms-text-size-adjust: 100% !important;\n                                     -webkit-font-smoothing: antialiased !important;\n                                     font-size: 16px;\n                                 }\n                                 a {\n                                     color: #382F2E;\n                                 }\n                         \n                                 p, h1 {\n                                     margin: 0;\n                                 }\n                         \n                                 p {\n                                     text-align: left;\n                                     font-weight: normal;\n                                     line-height: 19px;\n                                 }\n                         \n                                 a.link1 {\n                                     color: #382F2E;\n                                 }\n                         \n                                 a.link2 {\n                                     text-decoration: none;\n                                     color: #ffffff;\n                                 }\n                         \n                                 h2 {\n                                     text-align: left;\n                                     color: #222222;\n                                     font-size: 19px;\n                                     font-weight: normal;\n                                 }\n                         \n                                 div, p, ul, h1 {\n                                     margin: 0;\n                                 }\n                         \n                                 .bgBody {\n                                     background: #ffffff;\n                                 }\n                         \n                                   table thead tr th {\n                        border: 1px solid black;\n                        padding: 5px;\n                        margin: 2px;\n                        text-align: left;\n                    }\n            \n                    table tbody tr td {\n                        border: 1px solid black;\n                        padding: 5px;\n                        margin: 2px;\n                        text-align: left;\n                    }\n                    .block\n                           {\n                           display:block;\n                           }\n                           .none\n                           {\n                           display:none;\n                           }\n      					 .btn-confirm\n    					 {\n      					 background-color:#26327e !important;\n      					 margin-left:30% !important;\n      					 color: white!important;\n      					 border:none !important;\n      					 width:40% !important;\n      					 margin-top:5% !important;\n      					 height:42px !important;\n      					 }\n                         .btn-cancel\n                         {\n                         background-color:#26327e !important;\n      					 margin-left:30% !important;\n      					 color: white!important;\n      					 border:none !important;\n      					 width:38% !important;\n      					 margin-top:5% !important;\n      					 height:42px !important;\n                         }\n    \n                             </style>\n                         </head>\n                         <body style=\"background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;\">\n                                 <div class=\"contentEditableContainer contentTextEditable\">\n                                     <div class=\"contentEditable\">\n                                         <span>Dear @Model.FullName,</span>\n                                         <br />\n                                         <br />\n                                         \n                                      <p>\n                      A new @Model.jobType is created with MARBLELIFE <sup>&reg;</sup> at @Model.StartDate at @Model.Time with Representative @Model.techNames We look forward to seeing you and assisting you with your project. <br/>Have a great day!\n                                    </p>\n   								 <br />\n   								 <table style=\"border: 1px solid black;border-collapse:collapse;\">\n                                    <thead>\n                                        <tr>\n                                        <th style=\"border: 1px solid black;\">Job Title</th>\n           								 <th style=\"border: 1px solid black;\">Address</th>\n                                           <th style=\"border: 1px solid black;\">Phone Number</th>\n                                        </tr>\n                                    </thead>\n                                    <tbody>\n                                        <tr>\n                                        <td style=\"border: 1px solid black;\">@Model.jobTitle</td>\n                                            <td style=\"border: 1px solid black;\">@Model.Address</td>\n                                            <td style=\"border: 1px solid black;\">@Model.PhoneNumber</td>\n                                        </tr>\n                                    </tbody>\n            						\n                                </table> \n   								 \n                                            <br />\n                                             @foreach (var item in Model.TechList)\n                                         {\n                                     <p>\n                                         @item.TechName <div class=\"margin-left:10px\">\n                                         </div> <img class=\"@Model.display\" src=\"@item.src\" style=\"height: 100px; width: 100px;display:@Model.display\" />\n                                         <br/>\n                                     </p>\n                                     }\n      							   \n                     				<br />\n                                  <table style=\"border: 1px solid black;border-collapse:collapse;\">\n                                    <thead>\n                                        <tr>\n                                        <th style=\"border: 1px solid black;\">Representative/Technician Name</th>\n           								 <th style=\"border: 1px solid black;\">Start Date</th>\n           								 <th style=\"border: 1px solid black;\">End Date</th>\n                                            \n           								\n                                        </tr>\n                                    </thead>\n            						 @foreach(var item in Model.TechList) \n           						 { \n                                    <tbody>\n                                        <tr>\n                                            <td style=\"border: 1px solid black;\">@item.FirstName @item.LastName </td>\n                                            <td style=\"border: 1px solid black;\">@item.StartDate</td>\n           								 <td style=\"border: 1px solid black;\">@item.EndDate</td>\n                                        </tr>\n                                    </tbody>\n            						}\n                                </table> \n     						 \n      						 <br/>\n                                <br/>\n                     				Regards,\n                                        \n                                         <p>\n                                           @Model.Base.CompanyName<sup>&reg;</sup> <br />\n                                           @Model.AssigneePhone <br />\n                         				  </p>\n                                         \n                                     </div>\n                                  </div>\n                         </body>\n                          </html>\n     \n    ' WHERE (`Id` = '37');


UPDATE `emailtemplate` 
SET `Body`='
       <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                          <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.CompanyName</title><style type="text/css">body {
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
                     .block
                            {
                            display:block;
                            }
                            .none
                            {
                            display:none;
                            }
       					 .btn-confirm
     					 {
       					 background-color:#26327e !important;
       					 margin-left:30% !important;
       					 color: white!important;
       					 border:none !important;
       					 width:40% !important;
       					 margin-top:5% !important;
       					 height:42px !important;
       					 }
                          .btn-cancel
                          {
                          background-color:#26327e !important;
       					 margin-left:30% !important;
       					 color: white!important;
       					 border:none !important;
       					 width:38% !important;
       					 margin-top:5% !important;
       					 height:42px !important;
                          }
     
                              </style>
                          </head>
                          <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                                  <div class="contentEditableContainer contentTextEditable">
                                      <div class="contentEditable">
                                          <span>Dear @Model.FullName,</span>
                                          <br />
                                          <br />
                                          
                                       <p>
                       There is updation in your @Model.jobType which was scheduled with MARBLELIFE <sup>&reg;</sup> at @Model.StartDate at @Model.Time with Representative @Model.techNames We look forward to seeing you and assisting you with your project. <br/>Have a great day!
                                     </p>
    								 <br />
    								 <table style="border: 1px solid black;border-collapse:collapse;">
                                     <thead>
                                         <tr>
                                         <th style="border: 1px solid black;">Job Title</th>
            								 <th style="border: 1px solid black;">Address</th>
                                            <th style="border: 1px solid black;">Phone Number</th>
                                         </tr>
                                     </thead>
                                     <tbody>
                                         <tr>
                                         <td style="border: 1px solid black;">@Model.jobTitle</td>
                                             <td style="border: 1px solid black;">@Model.Address</td>
                                             <td style="border: 1px solid black;">@Model.PhoneNumber</td>
                                         </tr>
                                     </tbody>
             						
                                 </table> 
    								 
                                             <br />
                                              @foreach (var item in Model.TechList)
                                          {
                                      <p>
                                          @item.TechName <div class="margin-left:10px">
                                          </div> <img class="@Model.display" src="@item.src" style="height: 100px; width: 100px;display:@Model.display" />
                                          <br/>
                                      </p>
                                      }
       							   
                      				<br />
                                   <table style="border: 1px solid black;border-collapse:collapse;">
                                     <thead>
                                         <tr>
                                         <th style="border: 1px solid black;">Representative/Technician Name</th>
            								 <th style="border: 1px solid black;">Start Date</th>
            								 <th style="border: 1px solid black;">End Date</th>
                                             
            								
                                         </tr>
                                     </thead>
             						 @foreach(var item in Model.TechList) 
            						 { 
                                     <tbody>
                                         <tr>
                                             <td style="border: 1px solid black;">@item.FirstName @item.LastName </td>
                                             <td style="border: 1px solid black;">@item.StartDate</td>
            								 <td style="border: 1px solid black;">@item.EndDate</td>
                                         </tr>
                                     </tbody>
             						}
                                 </table> 
      						 
       						 <br/>
                                 <br/>
                      				Regards,
                                         
                                          <p>
                                            @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                            @Model.AssigneePhone <br />
                          				  </p>
                                          
                                      </div>
                                   </div>
                          </body>
                           </html>
      
     
' 
WHERE `Id`='38'