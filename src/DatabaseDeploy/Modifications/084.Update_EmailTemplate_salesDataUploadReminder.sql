UPDATE `EmailTemplate` SET `Body`=
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
                 <span>Dear @Model.FullName,</span>
                 <br>
                 <br />
                 <p>
                     This is to remind you, that we have not received Sales Data for given periods:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Start Date</th>
                                 <th style="border: 1px solid black;"> End Date</th>
                             </tr>
                         </thead>
 						 @{foreach(var item in Model.DateRange) { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.StartDate</td>
                                 <td style="border: 1px solid black;">@item.EndDate</td>
                             </tr>
                         </tbody>
 						}}
                     </table>
 
                     <br />
                     <br />
 
                     <b>Kindly upload the files as earliest as possible, to avoid Late Fee charges..</b>
                     <br /><br />
                     
                     Regards,
                     <br /><br />
 					<p>
                     @Model.Base.OwnerName<br>
 					@Model.Base.Designation<br>
 					@Model.Base.CompanyName <br>
 					@Model.Base.Phone <br>
 					</p>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 WHERE `Id`='5';


UPDATE `EmailTemplate` SET `Title`='LateFee  For SalesDataUpload', `Description`='LateFee  For SalesDataUpload', `Subject`='LateFee For SalesDataUpload | @Model.Franchisee' WHERE `Id`='8';
UPDATE `EmailTemplate` SET `Title`='LateFee  For Payment', `Description`='LateFee  For Payment', `Subject`='Late Fee  For Payment | @Model.Franchisee' WHERE `Id`='6';


UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> <html xmlns="http://www.w3.org/1999/xhtml"> <head>     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />     <title>@Model.Base.ApplicationName</title>     <style type="text/css">         body {             padding-left: 10px !important;             padding-right: 10px !important;             padding-top: 10px !important;             padding-bottom: 10px !important;             margin: 20px !important;             width: 600px;             border: solid 1px #B2C8D9;             -webkit-text-size-adjust: 100% !important;             -ms-text-size-adjust: 100% !important;             -webkit-font-smoothing: antialiased !important;             font-size: 16px;         }         a {             color: #382F2E;         }          p, h1 {             margin: 0;         }          p {             text-align: left;             font-weight: normal;             line-height: 19px;         }          a.link1 {             color: #382F2E;         }          a.link2 {             text-decoration: none;             color: #ffffff;         }          h2 {             text-align: left;             color: #222222;             font-size: 19px;             font-weight: normal;         }          div, p, ul, h1 {             margin: 0;         }          .bgBody {             background: #ffffff;         }          table thead tr th {             border: 1px solid black;             padding: 5px;             margin: 2px;             text-align: left;         }          table tbody tr td {             border: 1px solid black;             padding: 5px;             margin: 2px;             text-align: left;         }     </style> </head> <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">         <div class="contentEditableContainer contentTextEditable">             <div class="contentEditable">                 <span>Dear @Model.FullName,</span>                 <br>                 <br />                 <p>                    This is for your kind information, that as a result of delay in upload of SalesData, Late Fee has been charged. Please see the invoice listed below:                     <br>                     <br />                     <table style="border: 1px solid black;border-collapse:collapse;">                         <thead>                             <tr>                                 <th style="border: 1px solid black;">Invoice Number</th>                                 <th style="border: 1px solid black;"> Generated On</th>								 <th style="border: 1px solid black;">Upload Period</th>                                 <th style="border: 1px solid black;"> Amount</th>                                 <th style="border: 1px solid black;">Description</th>                             </tr>                         </thead> 						 <tbody>                             <tr>                                <td style="border: 1px solid black;">@Model.InvoiceId</td>                                <td style="border: 1px solid black;">@Model.ExpectedDate</td>								<td style="border: 1px solid black;">@Model.StartDate / @Model.EndDate</td> 								<td style="border: 1px solid black;">$ @Model.Amount</td> 								<td style="border: 1px solid black;">@Model.Description</td>                             </tr>                         </tbody> 						</table>                     <br />                     <br />                     <b>Kindly upload the files as earliest as possible, to avoid Late Fee charges..</b>                     <br>                     <br />                                         Regards,                     <br /><br /> 					<p>                     @Model.Base.OwnerName<br> 					@Model.Base.Designation<br> 					@Model.Base.CompanyName <br> 					@Model.Base.Phone <br> 					</p>                 </p>             </div>         </div> </body> </html>'
 WHERE `Id`='8';