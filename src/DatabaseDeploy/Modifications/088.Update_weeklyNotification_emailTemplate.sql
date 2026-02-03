UPDATE `makalu`.`emailtemplate` SET `Body`=
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
				@if(Model.WeeklyCollection.Count() <= 0)
				{
					  <span>No late fee has been charged to any Franchisee during <b>@Model.EndDate</b> to <b>@Model.StartDate</b>.</span>
					  <br /><br />
				}
 				@if(Model.WeeklyCollection.Count() > 0)
				{
					  <span>Following is the list of late Fee charged on franchisee invoices during <b>@Model.EndDate</b> to <b>@Model.StartDate</b>.</span>
						<br /><br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">ID#</th>
                                 <th style="border: 1px solid black;">Franchisee</th>
                                 <th style="border: 1px solid black;">Invoice#</th>
                                 <th style="border: 1px solid black;">Start Date</th>
                                 <th style="border: 1px solid black;">End Date</th>
                                 <th style="border: 1px solid black;">Due Date</th>
                                 <th style="border: 1px solid black;">Late Fee Type</th>
                                 <th style="border: 1px solid black;">Payable Late Fee</th>
                                 <th style="border: 1px solid black;">Status</th>
                             </tr>
                         </thead>
 						 @foreach(var item in Model.WeeklyCollection) 
						 { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.FranchiseeId</td>
                                 <td style="border: 1px solid black;">@item.Franchisee</td>
                                 <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.StartDate</td>
                                 <td style="border: 1px solid black;">@item.EndDate</td>
                                 <td style="border: 1px solid black;">@item.DueDate</td>
                                 <td style="border: 1px solid black;">@item.LateFeeType</td>
                                 <td style="border: 1px solid black;">$@item.LateFeeAmount</td>
                                 <td style="border: 1px solid black;">@item.Status</td>
                             </tr>
                         </tbody>
 						}
                     </table> 
                     <br />
                     <br /> 
				}
					
				@if(Model.PreviousCollection.Count() > 0)
				{
					  <span>Following is the list of late Fee charged on franchisee invoices during <b>@Model.EndDate</b> to <b>@Model.StartDate</b>.</span>
                     <br /><br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">ID#</th>
                                 <th style="border: 1px solid black;">Franchisee</th>
                                 <th style="border: 1px solid black;">Invoice#</th>
                                 <th style="border: 1px solid black;">Start Date</th>
                                 <th style="border: 1px solid black;">End Date</th>
                                 <th style="border: 1px solid black;">Due Date</th>
                                 <th style="border: 1px solid black;">Late Fee Type</th>
                                 <th style="border: 1px solid black;">Payable Late Fee</th>
                                 <th style="border: 1px solid black;">Status</th>
                             </tr>
                         </thead>
 						 @foreach(var item in Model.PreviousCollection) 
						 { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.FranchiseeId</td>
                                 <td style="border: 1px solid black;">@item.Franchisee</td>
                                 <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.StartDate</td>
                                 <td style="border: 1px solid black;">@item.EndDate</td>
                                 <td style="border: 1px solid black;">@item.DueDate</td>
                                 <td style="border: 1px solid black;">@item.LateFeeType</td>
                                 <td style="border: 1px solid black;">$@item.LateFeeAmount</td>
                                 <td style="border: 1px solid black;">@item.Status</td>
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
 </html>'
 WHERE `Id`='9';


 UPDATE `emailtemplate` SET `Body`=
 '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                  <span>Dear  @Model.Base.OwnerName,</span>
                  <br>
                  <br />
                  <p>
				  @if(Model.WeeklyCollection.Count() <= 0)
					{
					  <span>No unpaid Invoices for any Franchisee during <b>@Model.EndDate</b> to <b>@Model.StartDate</b>.</span>
					  <br /><br />
					}
 				@if(Model.WeeklyCollection.Count() > 0)
				{
  						<span>Following is the list of unpaid franchisee invoices generated during <b>@Model.EndDate</b> to <b>@Model.StartDate</b>.</span>
                      <br />
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">ID#</th>
                                  <th style="border: 1px solid black;">Franchisee</th>
                                  <th style="border: 1px solid black;">Invoice#</th>
                                  <th style="border: 1px solid black;">Start Date</th>
                                  <th style="border: 1px solid black;">End Date</th>
                                  <th style="border: 1px solid black;">Due Date</th>
								  <th style="border: 1px solid black;">Invoice Amount</th>
								  <th style="border: 1px solid black;">Late Fee Applicable</th>
                                  <th style="border: 1px solid black;">Payable Amount</th>
                              </tr>
                          </thead>
  						 @foreach(var item in Model.WeeklyCollection) 
						 { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.FranchiseeId</td>
                                  <td style="border: 1px solid black;">@item.Franchisee</td>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                  <td style="border: 1px solid black;">@item.StartDate</td>
                                  <td style="border: 1px solid black;">@item.EndDate</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
								  <td style="border: 1px solid black;">$@item.InvoiceAmount</td>
                                  <td style="border: 1px solid black;">@item.LateFeeApplicable</td>
                                  <td style="border: 1px solid black;">$@item.PayableAmount</td>
                             </tr>
                          </tbody>
  						}
                      </table>  
                      <br />
                      <br />  
				} 
				@if(Model.PreviousCollection.Count() > 0)
				{
  						<span>Following is the list of unpaid franchisee invoices generated Before <b>@Model.EndDate</b>.</span>
                      <br />
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">ID#</th>
                                  <th style="border: 1px solid black;">Franchisee</th>
                                  <th style="border: 1px solid black;">Invoice#</th>
                                  <th style="border: 1px solid black;">Start Date</th>
                                  <th style="border: 1px solid black;">End Date</th>
                                  <th style="border: 1px solid black;">Due Date</th>
								  <th style="border: 1px solid black;">Invoice Amount</th>
								  <th style="border: 1px solid black;">Late Fee Applicable</th>
                                  <th style="border: 1px solid black;">Payable Amount</th>
                              </tr>
                          </thead>
  						 @foreach(var item in Model.PreviousCollection) 
						 { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.FranchiseeId</td>
                                  <td style="border: 1px solid black;">@item.Franchisee</td>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                  <td style="border: 1px solid black;">@item.StartDate</td>
                                  <td style="border: 1px solid black;">@item.EndDate</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
								  <td style="border: 1px solid black;">$@item.InvoiceAmount</td>
                                  <td style="border: 1px solid black;">@item.LateFeeApplicable</td>
                                  <td style="border: 1px solid black;">$@item.PayableAmount</td>
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
  </html>'
  WHERE `Id`='10';

