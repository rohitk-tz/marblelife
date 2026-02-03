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
                       There has been an update in your @Model.jobType which was scheduled with MARBLELIFE <sup>&reg;</sup> with Representative @Model.techNames . We look forward to seeing you and assisting you with your project. <br/><br/>Have a great day!
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