Alter table emailtemplate
ADD Column `IsRequired` Bit(1) default b'1' after body;

UPDATE `makalu`.`emailtemplate` SET `IsRequired` = b'0' WHERE (`Id` = '29');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'TECH - Job Scheduled Confirmation' WHERE (`Id` = '30');

UPDATE `makalu`.`emailtemplate` SET `IsRequired` = b'0' WHERE (`Id` = '35');

DELETE FROM `makalu`.`emailtemplate` WHERE (`Id` = '79');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - Before After Images' WHERE (`Id` = '34');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - Before After Images' WHERE (`Id` = '74');

UPDATE `makalu`.`emailtemplate` SET `IsRequired` = b'0' WHERE (`Id` = '41');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'Before After Images to Franchisee' WHERE (`Id` = '42');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - New Job Scheduled' WHERE (`Id` = '37');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - Job Schedule Update' WHERE (`Id` = '38');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - New Job Scheduled' WHERE (`Id` = '76');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'CUSTOMER - Job Schedule Update' WHERE (`Id` = '77');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'Renewable Mail to Franchisee Before 9 months' WHERE (`Id` = '44');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'Renewable Mail to Franchisee Before 8 months' WHERE (`Id` = '45');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'Renewable Mail to Franchisee Before 8 months' WHERE (`Id` = '92');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'Personal Mail For Members', `Description` = 'Personal Mail For Members' WHERE (`Id` = '47');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'Job Invoice to Customer', `Description` = 'Job Invoice to Customer' WHERE (`Id` = '573');

UPDATE `makalu`.`emailtemplate` SET `Title` = 'Post Job Completion  Invoice to Customer', `Description` = 'Post Job Completion  Invoice to Customer' WHERE (`Id` = '574');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'Post Job Completion Invoice To SalesRep ', `Description` = 'Post Job Completion Invoice to Sales Rep' WHERE (`Id` = '575');
UPDATE `makalu`.`emailtemplate` SET `Title` = 'Post Job Completion Invoice To Admin', `Description` = 'Post Job Completion Customer To Admin' WHERE (`Id` = '576');

update emailtemplate
Set Body = '
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 <html xmlns="http://www.w3.org/1999/xhtml">
 <head>
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" F
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
        a 
		{
			color: #382F2E;
        }
		.block
        {
			display:block;
        }
        .none
        {
			display:none;
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
 				Thank you for uploading your @Model.Base.ApplicationName DATA for the period @Model.StartDate to @Model.EndDate.<br>
 				Please find listed below a copy of your Adfund/Royalty invoice amounts.
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Invoice Number</th>
                                 <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;"> Due Date</th>
                                 <th style="border: 1px solid black;"> Ad Fund</th>
                                 <th style="border: 1px solid black;"> Royalty</th>
                                 <th style="border: 1px solid black;">Total Amount</th>
                             </tr>
                         </thead>
 						 @{foreach(var item in Model.InvoiceDetailList) { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.GeneratedOn</td>
                                 <td style="border: 1px solid black;">@item.DueDate</td>
                                 <td style="border: 1px solid black;">$ @item.AdFund</td>
                                 <td style="border: 1px solid black;">$ @item.Royalty</td>
                                 <td style="border: 1px solid black;">$ @item.TotalPayment</td>
                             </tr>
                         </tbody>
 						}}
                     </table>
                     <br />
                     <br />
                     <b>Please insure payments are made by @Model.DueDate to avoid any Late Fees or Interest.</b>
                     <br /><br />
                     Thank you once again for taking the time to upload your data and keeping your account payment current.
                     <br>
                     <br />
 					<div style= "display: @Model.HasCustomSignature">
					@Model.Signature
					</div>
					<div style= "display: @Model.NotHasCustomSignature">
                     Regards,<br>
                     @Model.Base.OwnerName<br>
 					@Model.Base.Designation<br>
 					@Model.Base.CompanyName <br>
 					@Model.Base.Phone <br>
 					</div>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 Where  NotificationTypeId = 3 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                  <span>Estimado @Model.FullName,</span>
                  <br>
                  <br />
                  <p>
                TGracias por cargar sus DATOS AL INFORME DE REGAL?AS DE MARBLELIFE para el per?odo del @Model.StartDate to @Model.EndDate.<br>
                A continuaci?n encontrar? una copia de los montos de sus facturas de Adfund/Regal?as.
                      <br>
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Invoice Number</th>
                                  <th style="border: 1px solid black;"> Generated On</th>
                                  <th style="border: 1px solid black;"> Due Date</th>
                                  <th style="border: 1px solid black;"> Ad Fund</th>
                                  <th style="border: 1px solid black;"> Royalty</th>
                                  <th style="border: 1px solid black;">Total Amount</th>
                              </tr>
                          </thead>
                         @{foreach(var item in Model.InvoiceDetailList) { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                  <td style="border: 1px solid black;">@item.GeneratedOn</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
                                  <td style="border: 1px solid black;">$ @item.AdFund</td>
                                  <td style="border: 1px solid black;">$ @item.Royalty</td>
                                  <td style="border: 1px solid black;">$ @item.TotalPayment</td>
                              </tr>
                          </tbody>
                        }}
                      </table>
  
                      <br />
                      <br />
  
                      <b>Aseg?rese de que los pagos se realicen antes del @Model.DueDate para evitar cargos por demora o intereses.</b>
                      <br /><br />
                      Gracias una vez m?s por tomarse el tiempo de cargar sus datos y mantener el pago de su cuenta al d?a.
  
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                      <br /><br />
                    <p>
                      @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </p>
                    </div>                      
                  </p>
              </div>
          </div>
  </body>
  </html>'
Where NotificationTypeId = 3 and LanguageId = 250 and Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
         .block
        {
            display:block;
        }
        .none
        {
            display:none;
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
                This is to inform you that you have following pending payments.
                <br />
                Please see the details below:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Invoice Number</th>
                                 <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;"> Due Date</th>
                                 <th style="border: 1px solid black;"> Ad Fund</th>
                                 <th style="border: 1px solid black;"> Royalty</th>
                                 <th style="border: 1px solid black;">Payable Amount</th>
                             </tr>
                         </thead>
                        @{foreach(var item in Model.InvoiceDetailList) {                        
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.GeneratedOn</td>
                                 <td style="border: 1px solid black;">@item.DueDate</td>
                                 <td style="border: 1px solid black;">$ @item.AdFund</td>
                                 <td style="border: 1px solid black;">$ @item.Royalty</td>
                                 <td style="border: 1px solid black;">$ @item.Amount</td>
                             </tr>
                         </tbody>
                        }}
                     </table>
                     <br />
                     <br />
 
                     <b>Please note, in case the payments are not made per the due date, a Late Fee will be applied. Further Non Payment will result in an interest amount being applied on daily basis.</b>
                     <br /><br />
                     Kindly make the payment as per due date to avoid these charges.
                     <br /> <br />
                     <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style= "display: @Model.NotHasCustomSignature">
                    Regards,
                     <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 Where NotificationTypeId = 4 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
      </style>
  </head>
  <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
          <div class="contentEditableContainer contentTextEditable">
              <div class="contentEditable">
                  <span>Estimado @Model.FullName,</span>
                  <br>
                  <br />
                  <p>
                Esto es para informarle que tiene los siguientes pagos pendientes.
                <br />
                Por favor, vea los detalles a continuaci?n:
                      <br>
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Invoice Number</th>
                                  <th style="border: 1px solid black;"> Generated On</th>
                                  <th style="border: 1px solid black;"> Due Date</th>
                                  <th style="border: 1px solid black;"> Ad Fund</th>
                                  <th style="border: 1px solid black;"> Royalty</th>
                                  <th style="border: 1px solid black;">Payable Amount</th>
                              </tr>
                          </thead>
                        @{foreach(var item in Model.InvoiceDetailList) {                        
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                  <td style="border: 1px solid black;">@item.GeneratedOn</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
                                  <td style="border: 1px solid black;">$ @item.AdFund</td>
                                  <td style="border: 1px solid black;">$ @item.Royalty</td>
                                  <td style="border: 1px solid black;">$ @item.Amount</td>
                              </tr>
                          </tbody>
                        }}
                      </table>
                     <br />
                      <br />
  
                      <b>Tenga en cuenta que, en caso de que los pagos no se realicen en la fecha de vencimiento, se aplicar? un cargo por pago atrasado. Falta de pago adicional resultar? en un monto de inter?s que se aplicar? diariamente.</b>
                      <br /><br />
                      Por favor haga el pago antes de la fecha de vencimiento para evitar estos cargos.
                      <br /> <br />
                      
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style= "display: @Model.NotHasCustomSignature">
                    Saludos,
                      <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 4 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
          .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
  
                      <b>Kindly upload the files as earliest as possible, to avoid Late Fee charges.</b>
                      <br /><br />
                      
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div class= "@Model.NotHasCustomSignature">
                      Regards,
                      <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 5 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
           .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
       </style>
   </head>
   <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
           <div class="contentEditableContainer contentTextEditable">
               <div class="contentEditable">
                   <span>Estimado @Model.FullName,</span>
                   <br>
                   <br />
                   <p>
                       Esto es para recordarle que no hemos recibido datos de ventas para este per?odo:
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
   
                       <b>Por favor cargue los archivos lo antes posible para evitar cargos por pago atrasado.</b>
                       <br /><br />
                       
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                       Saludos,
                       <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                   </p>
               </div>
           </div>
   </body>
   </html>'
 Where NotificationTypeId = 5 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                    This is for your kind information, that as a result of non-payment of Invoice Number – @Model.InvoiceId, Late Fee has been charged. Please see the invoice listed below:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Invoice Number</th>
                                 <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;"> Amount</th>
                                 <th style="border: 1px solid black;">Description</th>
                             </tr>
                         </thead>
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                 <td style="border: 1px solid black;">@Model.ExpectedDate</td>
                                <td style="border: 1px solid black;">$ @Model.Amount</td>
                                <td style="border: 1px solid black;">@Model.Description</td>
                             </tr>
                         </tbody>
                        </table>
 
                    <br />
                     <br />
 
                     <b>Please note, in case the payments are not made per the due date, a Late Fee will be applied. Further Non Payment will result in an interest amount being applied on daily basis.</b>
                     <br /><br />
                     Kindly make the payment as per due date to avoid these charges.
 
                     <br>
                     <br />
                     
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                     Regards,
                     <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 Where NotificationTypeId = 6 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
      </style>
  </head>
  <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
          <div class="contentEditableContainer contentTextEditable">
              <div class="contentEditable">
                  <span>Estimado @Model.FullName,</span>
                  <br>
                  <br />
                  <p>
                     Le escribimos para informarle, que como resultado de la falta de pago del n?mero de factura:- @Model.InvoiceId, se ha cobrado un Cargo por Pago atrasado. Consulte la factura que se detalla a continuaci?n:
                      <br>
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Invoice Number</th>
                                  <th style="border: 1px solid black;"> Generated On</th>
                                  <th style="border: 1px solid black;"> Amount</th>
                                  <th style="border: 1px solid black;">Description</th>
                              </tr>
                          </thead>
                         <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                  <td style="border: 1px solid black;">@Model.ExpectedDate</td>
                                <td style="border: 1px solid black;">$ @Model.Amount</td>
                                <td style="border: 1px solid black;">@Model.Description</td>
                              </tr>
                          </tbody>
                        </table>
  
                     <br />
                      <br />  
                      <b>Tenga en cuenta que, en caso de que los pagos no se realicen en la fecha de vencimiento, se aplicar? un cargo por pago atrasado. Falta de pago adicional resultar? en un monto de inter?s que se aplicar? diariamente.</b>
                      <br /><br />
                      Por favor haga el pago antes de la fecha de vencimiento para evitar estos cargos.  
                      <br>
                      <br />                      
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                      Saludos,
                      <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 6 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                This is to confirm that we have received the payment of amount $ @Model.Amount, against Invoice Number - @Model.InvoiceId.
                <br />
                Please see the invoice details below:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Invoice Number</th>
                                 <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;"> Due Date</th>
                                 <th style="border: 1px solid black;"> Ad Fund</th>
                                 <th style="border: 1px solid black;"> Royalty</th>
                                 <th style="border: 1px solid black;">Total Amount</th>
                             </tr>
                         </thead>                       
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                 <td style="border: 1px solid black;">@Model.GeneratedOn</td>
                                 <td style="border: 1px solid black;">@Model.DueDate</td>
                                 <td style="border: 1px solid black;">$ @Model.AdFund</td>
                                 <td style="border: 1px solid black;">$ @Model.Royalty</td>
                                 <td style="border: 1px solid black;">$ @Model.Amount</td>
                             </tr>
                         </tbody>
                     </table>
                    <br />
                     <h4>Please find Payment Details listed below:</h4>
                    <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Payment Number</th>
                                 <th style="border: 1px solid black;"> Payment Mode</th>
                                 <th style="border: 1px solid black;"> Date Of payment</th>
                                 <th style="border: 1px solid black;"> Amount</th>
                            </tr>
                         </thead>
                         @{foreach(var item in Model.Payments) { 
                         <tbody>
                             <tr>
                                 <td style="border: 1px solid black;">@item.Id</td>
                                 <td style="border: 1px solid black;">@item.InstrumentType</td>
                                 <td style="border: 1px solid black;">@item.Date</td>
                                 <td style="border: 1px solid black;">$ @item.Amount</td>
                             </tr>
                         </tbody>
                        }}
                     </table>                    
                     <br>
                     <br />
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                     Regards,
                     <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 Where NotificationTypeId = 7 and LanguageId = 249 ANd Id>0;
 
update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
      </style>
  </head>
  <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
          <div class="contentEditableContainer contentTextEditable">
              <div class="contentEditable">
                  <span>Estimado @Model.FullName,</span>
                  <br>
                  <br />
                  <p>
                Esto es para confirmar que hemos recibido el pago de la cantidad de $ 85.54, para el n?mero de factura - @Model.InvoiceId.
                <br />
                Consulte los detalles de la factura a continuaci?n:
                      <br>
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Invoice Number</th>
                                  <th style="border: 1px solid black;"> Generated On</th>
                                  <th style="border: 1px solid black;"> Due Date</th>
                                  <th style="border: 1px solid black;"> Ad Fund</th>
                                  <th style="border: 1px solid black;"> Royalty</th>
                                  <th style="border: 1px solid black;">Total Amount</th>
                              </tr>
                          </thead>                      
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                  <td style="border: 1px solid black;">@Model.GeneratedOn</td>
                                  <td style="border: 1px solid black;">@Model.DueDate</td>
                                  <td>$ @Model.AdFund</td>
                                  <td>$ @Model.Royalty</td>
                                  <td style="border: 1px solid black;">$ @Model.Amount</td>
                              </tr>
                          </tbody>
                      </table>
                    <br />
                     <h4>Please find Payment Details listed below:</h4>
                    <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Payment Number</th>
                                  <th style="border: 1px solid black;"> Payment Mode</th>
                                  <th style="border: 1px solid black;"> Date Of payment</th>
                                  <th style="border: 1px solid black;"> Amount</th>
                             </tr>
                          </thead>
                         @{foreach(var item in Model.Payments) { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.Id</td>
                                  <td style="border: 1px solid black;">@item.InstrumentType</td>
                                  <td style="border: 1px solid black;">@item.Date</td>
                                  <td style="border: 1px solid black;">$ @item.Amount</td>
                              </tr>
                          </tbody>
                        }}
                      </table>    
                      <br>
                      <br />
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                      Saludos,
                      <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 7 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
         .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                    This is for your kind information, that as a result of delay in upload of SalesData, Late Fee has been charged. Please see the invoice listed below:
                     <br>
                     <br />
                     <table style="border: 1px solid black;border-collapse:collapse;">
                         <thead>
                             <tr>
                                 <th style="border: 1px solid black;">Invoice Number</th>
                                 <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;">Upload Period</th>
                                 <th style="border: 1px solid black;"> Amount</th>
                                 <th style="border: 1px solid black;">Description</th>
                             </tr>
                         </thead>
                         <tbody>
                             <tr>
                                <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                <td style="border: 1px solid black;">@Model.ExpectedDate</td>
                                <td style="border: 1px solid black;">@Model.StartDate / @Model.EndDate</td>
                                <td style="border: 1px solid black;">$ @Model.Amount</td>
                                <td style="border: 1px solid black;">@Model.Description</td>
                             </tr>
                         </tbody>
                        </table>
 
                    <br />
                     <br />
 
                    <b>Kindly upload the files as earliest as possible, to avoid Late Fee charges..</b>
                     <br>
                     <br />
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                     <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                 </p>
             </div>
         </div>
 </body>
 </html>'
 Where NotificationTypeId = 8 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                     Esto es para informarle, que como resultado de la demora en la carga de datos de venta, se ha cobrado un cargo por pago atrasado. Consulte la factura que se detalla a continuaci?n:
                      <br>
                      <br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Invoice Number</th>
                                  <th style="border: 1px solid black;"> Generated On</th>
                                 <th style="border: 1px solid black;">Upload Period</th>
                                  <th style="border: 1px solid black;"> Amount</th>
                                  <th style="border: 1px solid black;">Description</th>
                              </tr>
                          </thead>
                         <tbody>
                              <tr>
                                 <td style="border: 1px solid black;">@Model.InvoiceId</td>
                                 <td style="border: 1px solid black;">@Model.ExpectedDate</td>
                                <td style="border: 1px solid black;">@Model.StartDate / @Model.EndDate</td>
                                <td style="border: 1px solid black;">$ @Model.Amount</td>
                                <td style="border: 1px solid black;">@Model.Description</td>
                              </tr>
                          </tbody>
                        </table>
  
                     <br />
                      <br />
  
                     <b>Cargue los archivos lo antes posible para evitar Cargos por Pago Atrasado.</b>
                      <br>
                      <br />
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                     Saludos,
                      <br /><br />
                    @Model.Base.OwnerName<br>
                    @Model.Base.Designation<br>
                    @Model.Base.CompanyName <br>
                    @Model.Base.Phone <br>
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 8 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
         .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                      <span>No late fee has been charged to any Franchisee during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
                      <br /><br />
                }
                @if(Model.WeeklyCollection.Count() > 0)
                {
                      <span>Following is the list of late Fee charged to franchisee during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
                        <br /><br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Franchisee</th>
                                  <th style="border: 1px solid black;">Invoice#</th>
                                 <th style="border: 1px solid black;">Start Date</th>
                                 <th style="border: 1px solid black;">End Date</th>
                                  <th style="border: 1px solid black;">Due Date</th>
                                  <th style="border: 1px solid black;">Generated On</th>
                                 <th style="border: 1px solid black;">Late Fee Type</th>
                                  <th style="border: 1px solid black;">Payable Late Fee</th>
                                  <th style="border: 1px solid black;">Status</th>
                              </tr>
                          </thead>
                         @foreach(var item in Model.WeeklyCollection) 
                         { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.Franchisee</td>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.StartDate</td>
                                  <td style="border: 1px solid black;">@item.EndDate</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
                                  <td style="border: 1px solid black;">@item.GeneratedOn</td>
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
                
                @if(Model.PreviousCollection.Count() <= 0)
                {
                      <span>There are no Unpaid late fee invoices before <b>@Model.StartDate</b>.</span>
                      <br /><br />
                }   
                @if(Model.PreviousCollection.Count() > 0)
                {
                      <span>Following is the list of late fee charged to franchisee that has not been paid yet.</span>
                      <br /><br />
                      <table style="border: 1px solid black;border-collapse:collapse;">
                          <thead>
                              <tr>
                                  <th style="border: 1px solid black;">Franchisee</th>
                                  <th style="border: 1px solid black;">Invoice#</th>
                                 <th style="border: 1px solid black;">Start Date</th>
                                 <th style="border: 1px solid black;">End Date</th>
                                  <th style="border: 1px solid black;">Due Date</th>
                                 <th style="border: 1px solid black;">Generated On</th>
                                  <th style="border: 1px solid black;">Late Fee Type</th>
                                  <th style="border: 1px solid black;">Payable Late Fee</th>
                                  <th style="border: 1px solid black;">Status</th>
                              </tr>
                          </thead>
                         @foreach(var item in Model.PreviousCollection) 
                         { 
                          <tbody>
                              <tr>
                                  <td style="border: 1px solid black;">@item.Franchisee</td>
                                  <td style="border: 1px solid black;">@item.InvoiceId</td>
                                 <td style="border: 1px solid black;">@item.StartDate</td>
                                  <td style="border: 1px solid black;">@item.EndDate</td>
                                  <td style="border: 1px solid black;">@item.DueDate</td>
                                  <td style="border: 1px solid black;">@item.GeneratedOn</td>
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
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">  
                    Regards,
                      <br /><br />
                    @Model.Base.CompanyName 
                    </div>
                  </p>
              </div>
          </div>
  </body>
  </html>'
 Where NotificationTypeId = 9 and LanguageId = 249 ANd Id>0;
 
update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
            .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                      <span>No unpaid Invoices for any Franchisee during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
                      <br /><br />
                    }
                @if(Model.WeeklyCollection.Count() > 0)
                {
                            <span>Following is the list of unpaid invoices generated during <b>@Model.StartDate</b> to <b>@Model.EndDate</b>.</span>
                        <br />
                        <br />
                        <table style="border: 1px solid black;border-collapse:collapse;">
                            <thead>
                                <tr>
                                    <th style="border: 1px solid black;">Invoice#</th>
                                    <th style="border: 1px solid black;">Franchisee</th>
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
                                    <td style="border: 1px solid black;">@item.InvoiceId</td>
                                    <td style="border: 1px solid black;">@item.Franchisee</td>
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
                 @if(Model.PreviousCollection.Count() <= 0)
                    {
                      <span>No unpaid Invoices for any Franchisee before <b>@Model.StartDate</b>.</span>
                      <br /><br />
                    }
                @if(Model.PreviousCollection.Count() > 0)
                {
                            <span>Following is the list of unpaid invoices generated Before <b>@Model.StartDate</b>.</span>
                        <br />
                        <br />
                        <table style="border: 1px solid black;border-collapse:collapse;">
                            <thead>
                                <tr>
                                    <th style="border: 1px solid black;">Invoice#</th>
                                  <th style="border: 1px solid black;">Franchisee</th>
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
                                    <td style="border: 1px solid black;">@item.InvoiceId</td>
                                    <td style="border: 1px solid black;">@item.Franchisee</td>
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
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">  
                        Regards,
                        <br /><br />
                        @Model.Base.CompanyName <br>
                        </div>
                    </p>
                </div>
            </div>
    </body>
    </html>'
 Where NotificationTypeId = 10 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
               .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
           </style>
       </head>
       <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
               <div class="contentEditableContainer contentTextEditable">
                   <div class="contentEditable">
                       <font size="3" color="#454545">
                         Dear  @Model.Base.OwnerName,<br /><br />
                        Please find attached the list of Franchisee(s), who have failed to upload their Sales data on time.<br /><br />
                        </font>     
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,<br /><br />
                        @Model.Base.CompanyName <br>
                        @Model.Base.Phone <br>
                        </div>
                                 
                        <br>
                         <br /> 
                       </p>                   
                   </div>
               </div>
       </body>
       </html>'
 Where NotificationTypeId = 15 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                .block
            {
                display:block;
            }
            .none
            {
                display:none;
            }
            </style>
        </head>
        <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                <div class="contentEditableContainer contentTextEditable">
                    <div class="contentEditable">
                        <font size="3" color="#454545">
                         Estimada  @Model.Base.OwnerName,<br /><br />
                                Adjunto encontrar? la lista de franquiciados que no han cargado sus Datos de Venta a tiempo.<br /><br />
                         </font>        
                        <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature"> 
                    Saludos,<br /><br />
                        @Model.Base.CompanyName <br>
                        @Model.Base.Phone <br>
                        </div>        
                        <br>
                          <br />    
                        </p>                  
                    </div>
                </div>
        </body>
        </html>'
 Where NotificationTypeId = 15 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '
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
                    This is to confirm your appointment with MARBLELIFE <sup>&reg;</sup> at @Model.StartDate at @Model.Time with Representative @Model.techNames We look forward to seeing you and assisting you with your project. <br/><br/>Have a great day!
                                  </p>
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
      <div style="margin-top: 2%;">
    <a class="btn-confirm" style="background-color:#26327e;color:#FFFFFF;border:none;width:40%;margin-top:5%;height:42px;text-align:center;vertical-align:middle;display:table-cell;text-decoration: none;"
    href="@Model.ConfirmUrl" target="_blank">Confirm Your Appointment</a> <br/>
    </div>
  
  <div style="margin-top:2%;">
   <a class="btn-cancel" style="background-color:#26327e;color:#FFFFFF;border:none;width:38% !important;margin-top:5%;height:42px;text-align:center;vertical-align:middle;display:table-cell;text-decoration: none;"
    href="@Model.CancleUrl" target="_blank">Cancel Your Appointment</a>
  </div>
    <br/>
                              <br/>
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                                     
                                       <p>
                                         @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                         @Model.AssigneePhone <br />
                         </p>
                    </div>  
                                   </div>
                                </div>
                        </body>
                        </html>'
 Where NotificationTypeId = 25 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '
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
                                        <span>Estimado @Model.FullName,</span>
                                        <br />
                                        <br />
                                        
                                     <p>
                     Esto es para confirmar su cita con MARBLELIFE <sup>&reg;</sup> el @Model.StartDate el @Model.Time con el Representante @Model.techNames Esperamos verte y ayudarte con tu proyecto. <br/><br/>?Qu? tengas un lindo d?a!
                                   </p>
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
       <div style="margin-top: 2%;">
     <a class="btn-confirm" style="background-color:#26327e;color:#FFFFFF;border:none;width:40%;margin-top:5%;height:42px;text-align:center;vertical-align:middle;display:table-cell;text-decoration: none;"
     href="@Model.ConfirmUrl" target="_blank">Confirme su cita</a> <br/>
     </div>
   
   <div style="margin-top:2%;">
    <a class="btn-cancel" style="background-color:#26327e;color:#FFFFFF;border:none;width:38% !important;margin-top:5%;height:42px;text-align:center;vertical-align:middle;display:table-cell;text-decoration: none;"
     href="@Model.CancleUrl" target="_blank">Cancele su cita</a>
   </div>
     <br/>
                               <br/>
                               <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                                      
                                        <p>
                                          @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                          @Model.AssigneePhone <br />
                          </p>
                                       
                    </div>
                                    </div>
                                 </div>
                         </body>
                         </html>'
 Where NotificationTypeId = 25 and LanguageId = 250 ANd Id>0;
 
update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                  .block
            {
                display:block;
            }
            .none
            {
                display:none;
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
                      We have a LAST MINUTE @Model.jobTypeName that has been added to your schedule with details below:
                      <br />
                                  <table style="border: 1px solid black;border-collapse:collapse;">
                                   <thead>
                                       <tr>
                                       <th style="border: 1px solid black;">Customer Name</th>
                                        <th style="border: 1px solid black;">Start Date</th>
                                        <th style="border: 1px solid black;">End Date</th>
                                         <th style="border: 1px solid black;">Address</th>
                                         
                                       </tr>
                                   </thead>
                                   <tbody>
                                       <tr>
                                           <td style="border: 1px solid black;">@Model.FullName </td>
                                           <td style="border: 1px solid black;"> @Model.StartDate</td>
                                            <td style="border: 1px solid black;"> @Model.EndDate</td>
                                           <td style="border: 1px solid black;">@Model.Address</td>
                                         
                                       </tr>
                                   </tbody>
                                   </table>
                                   <br/> 
                                 <span> Please <a href="@Model.LinkUrl"> click here</a> to view the @Model.jobTypeName.</span><br/>
                      Good Luck!
                      </p>
                       <br/>
                  </div>
                  <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">                    
                      Regards,
                      <br/><br/>
                    @Model.AssigneeName<br />
                     @Model.Base.CompanyName <br />
                         @Model.AssigneePhone <br />
                    </div>          
              </div>          
          </body>
          </html>'
 Where NotificationTypeId = 30 and LanguageId = 249 ANd Id>0;
 
update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                  <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                          display:none
                          }
                      </style>
                  </head>
                  <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                          <div class="contentEditableContainer contentTextEditable">
                              <div class="contentEditable">
                                  <span>Dear @Model.TechName,</span>
                                  <br>
                                  <br />
                                  <p>
                                  
                                  This is to inform you that a @Model.jobTypeName with Customer @Model.FullName has been re-assigned to you. This means the customer may have initially received a communication indicating someone else was going to be coming, but was just sent a 
                               communication indicating that their @Model.jobTypeName has been re-assigned to yourself and with details as below: 
                                      <br/>
                                     <table style="border: 1px solid black;border-collapse:collapse;">
                                      <thead>
                                          <tr>
                                          <th style="border: 1px solid black;">Customer Name</th>
                                           <th style="border: 1px solid black;">Start Date</th>
                                           <th style="border: 1px solid black;">End Date</th>
                                             <th style="border: 1px solid black;">Address</th>
                                             
                                          </tr>
                                      </thead>
                                      <tbody>
                                          <tr>
                                              <td style="border: 1px solid black;">@Model.FullName </td>
                                              <td style="border: 1px solid black;"> @Model.StartDate</td>
                                               <td style="border: 1px solid black;"> @Model.EndDate</td>
                                              <td style="border: 1px solid black;">@Model.Address</td>
                                             
                                          </tr>
                                      </tbody>
                                      </table>
                                       <br />
                                <br/>
                               <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPTION</b> - <span>@Model.Description</span></span>
                               <span> <br/><br/>Please <a href="@Model.linkUrl"> click here</a> to view the @Model.jobTypeName .<span>
                                    <br />    <br/>              
                                Have a great visit!
                                      <br>
                                      <br />                  
                                      <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,<p>
                    @Model.Base.CompanyName<sup>&reg;<sup><br />
                     @Model.AssigneePhone <br /></p>
                    </div>
                                  </p>
                              </div>
                           </div>
                  </body>
                  </html>'
 Where NotificationTypeId = 31 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '
       <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                   <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                           display:none
                           }
                       </style>
                   </head>
                   <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                           <div class="contentEditableContainer contentTextEditable">
                               <div class="contentEditable">
                                   <span>Estimada @Model.TechName,</span>
                                   <br>
                                   <br />
                                   <p>
                                   
                                   Le informamos que se le ha reasignado un @Model.jobTypeName el cliente @Model.FullName. Esto significa que el cliente puede haber recibido 
 inicialmente un mensaje que indicaba que otra persona vendr?a, pero acaba de recibir un mensaje que indica que su trabajo se ha reasignado a usted, con los detalles a
  continuaci?n:
                                       <br/>
                                      <table style="border: 1px solid black;border-collapse:collapse;">
                                       <thead>
                                           <tr>
                                           <th style="border: 1px solid black;">Customer Name</th>
                                            <th style="border: 1px solid black;">Start Date</th>
                                            <th style="border: 1px solid black;">End Date</th>
                                             <th style="border: 1px solid black;">Address</th>
                                             
                                           </tr>
                                       </thead>
                                       <tbody>
                                           <tr>
                                               <td style="border: 1px solid black;">@Model.FullName </td>
                                               <td style="border: 1px solid black;"> @Model.StartDate</td>
                                                <td style="border: 1px solid black;"> @Model.EndDate</td>
                                               <td style="border: 1px solid black;">@Model.Address</td>
                                             
                                           </tr>
                                       </tbody>
                                       </table>
                                        <br />
                                 <br/>
                                <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCI?N</b> - <span>@Model.Description</span></span>
                                <span> <br/><br/>Please <a href="@Model.linkUrl"> Haga clic aqu?</a> para ver el trabajo. .<span>
                                     <br />    <br/>              
                                    ?Que tengas una gran visita!
                                       <br>
                                       <br />                  
                                       <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                                    <p>
                                     @Model.Base.CompanyName<sup>&reg;<sup><br />
                                     @Model.AssigneePhone <br />
                                    </p>
                    </div>
                                       
                                   </p>
                               </div>
                            </div>
                   </body>
                   </html>'
 Where NotificationTypeId = 31 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                           .block
                           {
                           display:block;
                           }
                            .none
                           {
                           display:none;
                           }
                       </style>
                   </head>
                   <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                       <div class="contentEditableContainer contentTextEditable">
                           <div class="contentEditable">
                               <span>Dear @Model.TechName,</span>
                               <br />
                               <br />
                               <p>
                               We have a LAST MINUTE @Model.jobTypeName that has been added to your schedule with details below:<br/>
                                <table style="border: 1px solid black;border-collapse:collapse;">
                                      <thead>
                                          <tr>
                                          <th style="border: 1px solid black;">Customer Name</th>
                                           <th style="border: 1px solid black;">Start Date</th>
                                           <th style="border: 1px solid black;">End Date</th>
                                             <th style="border: 1px solid black;">Address</th>
                                             
                                          </tr>
                                      </thead>
                                      <tbody>
                                          <tr>
                                              <td style="border: 1px solid black;">@Model.FullName </td>
                                              <td style="border: 1px solid black;"> @Model.StartDate</td>
                                               <td style="border: 1px solid black;"> @Model.EndDate</td>
                                              <td style="border: 1px solid black;">@Model.Address</td>
                                             
                                          </tr>
                                      </tbody>
                                      </table>
                               <br />
                                <br/>
                               <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPTION</b> - <span>@Model.Description</span></span>
                              <span> <br/><br/>Please <a href="@Model.LinkUrl"> click here</a> to view the @Model.jobTypeName .<span>
                               <br />    <br/>              
                                   Have a great day!</br>
                               Good Luck!
                               </p>
                                <br/>
                                <div style="display: "@Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: "@Model.NotHasCustomSignature">
                    Regards,
                           </div>
                           <p>
                                      @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                      @Model.AssigneePhone <br />
                           </p>
                    </div>                   
                       </div>
                   
                   </body>
                   </html>'
 Where NotificationTypeId = 32 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                            .block
                            {
                            display:block;
                            }
                             .none
                            {
                            display:none;
                            }
                        </style>
                    </head>
                    <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                        <div class="contentEditableContainer contentTextEditable">
                            <div class="contentEditable">
                                <span>Dear @Model.TechName,</span>
                                <br />
                                <br />
                                <p>
                                Tenemos un DE ?LTIMA HORA  @Model.jobTypeName que se ha agregado a su horario con los detalles a continuaci?n:<br/>
                                 <table style="border: 1px solid black;border-collapse:collapse;">
                                       <thead>
                                           <tr>
                                           <th style="border: 1px solid black;">Customer Name</th>
                                            <th style="border: 1px solid black;">Start Date</th>
                                            <th style="border: 1px solid black;">End Date</th>
                                             <th style="border: 1px solid black;">Address</th>
                                             
                                           </tr>
                                       </thead>
                                       <tbody>
                                           <tr>
                                               <td style="border: 1px solid black;">@Model.FullName </td>
                                               <td style="border: 1px solid black;"> @Model.StartDate</td>
                                                <td style="border: 1px solid black;"> @Model.EndDate</td>
                                               <td style="border: 1px solid black;">@Model.Address</td>
                                             
                                           </tr>
                                       </tbody>
                                       </table>
                                <br />
                                 <br/>
                                <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCI?N</b> - <span>@Model.Description</span></span>
                               <span> <br/><br/>Please <a href="@Model.LinkUrl"> Haga clic aqu? </a> para ver el @Model.jobTypeName .<span>
                                <br />    <br/>              
                                    ?Qu? tenga un lindo d?a!</br>
                                ?Buena suerte!
                                </p>
                                 <br/>
                                 <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                            </div>
                            <p>
                                       @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                       @Model.AssigneePhone <br />
                            </p>
                    </div>
                        </div>
                    </body>
                    </html>'
 Where NotificationTypeId = 32 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                           Your MARBLELIFE PROJECT has been scheduled with MARBLELIFE <sup>&reg;</sup> @Model.techDesignation  @Model.techNames  We look forward to seeing you and assisting you with your project. <br/><br/>Have a great day!
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
    
             
                           <br />
                                       <table style="border: 1px solid black;border-collapse:collapse;">
                                         <thead>
                                             <tr>
                                             <th style="border: 1px solid black;">Representative/Technician Name</th>
                 <th style="border: 1px solid black;">Start Date</th>
                 
                                                 
                 
                                             </tr>
                                         </thead>
                 @foreach(var item in Model.TechList) 
                 { 
                                         <tbody>
                                             <tr>
                                                 <td style="border: 1px solid black;">@item.FirstName @item.LastName </td>
                                                 <td style="border: 1px solid black;">@item.StartDate</td>
                 
                                             </tr>
                                         </tbody>
                 }
                                     </table> 
           
           <br/>
                                     <br/>
                                     <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                                             
                                              <p>
                                                @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                @Model.AssigneePhone <br />
                                </p>
                    </div>
                           
                                              
                                          </div>
                                       </div>
                              </body>
                                </html>'
 Where NotificationTypeId = 37 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                                               <span>Estimada @Model.FullName,</span>
                                               <br />
                                               <br />
                                               
                                            <p>
                            Su PROYECTO MARBLELIFE ha sido programado con el t?cnico/representante de ventas de MARBLELIFE ? @Model.techDesignation  @Model.techNames  Esperamos verle pronto y ayudarle con su proyecto. <br/><br/>?Qu? tenga un lindo d?a!
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
     
              
                            <br />
                                        <table style="border: 1px solid black;border-collapse:collapse;">
                                          <thead>
                                              <tr>
                                              <th style="border: 1px solid black;">Representative/Technician Name</th>
                  <th style="border: 1px solid black;">Start Date</th>
                  
                                                  
                  
                                              </tr>
                                          </thead>
                  @foreach(var item in Model.TechList) 
                  { 
                                          <tbody>
                                              <tr>
                                                  <td style="border: 1px solid black;">@item.FirstName @item.LastName </td>
                                                  <td style="border: 1px solid black;">@item.StartDate</td>
                  
                                              </tr>
                                          </tbody>
                  }
                                      </table> 
            
            <br/>
                                      <br/>
                                      <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                                              
                                               <p>
                                                 @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                 @Model.AssigneePhone <br />
                                 </p>
                    </div>
                            
                                               
                                           </div>
                                        </div>
                               </body>
                                 </html>'
 Where NotificationTypeId = 37 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                            There has been an update in your MARBLELIFE PROJECT scheduled with MARBLELIFE @Model.techDesignation @Model.techNames . <br/> We look forward to assisting you with your project.<span> <br/><br/>Please <a href="@Model.LinkUrl"> click here</a> to view the @Model.jobType .<span> <br/><br/>Have a great day!
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
                                                 
                                                   
                                                
                                               </tr>
                                           </thead>
                                         @foreach(var item in Model.TechList) 
                                         { 
                                           <tbody>
                                               <tr>
                                                   <td style="border: 1px solid black;">@item.FirstName @item.LastName </td>
                                                   <td style="border: 1px solid black;">@item.StartDate</td>
                                                
                                               </tr>
                                           </tbody>
                                        }
                                       </table> 
                                     
                                     <br/>
                                       <br/>
                                            <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                                
                                            </div>
                                         </div>
                                </body>
                                  </html>'
 Where NotificationTypeId = 38 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                             Ha habido una actualizaci?n en su PROYECTO MARBLELIFE programada con los t?cnicos/representantes de ventas de MARBLELIFE @Model.techDesignation @Model.techNames . <br/> Esperamos poder ayudarle con su proyecto.<span> <br/><br/>Please <a href="@Model.LinkUrl"> Haga clic aqu? </a> para ver el @Model.jobType .<span> <br/><br/>?Qu? tenga un lindo d?a!
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
                                                 
                                                    
                                                
                                                </tr>
                                            </thead>
                                             @foreach(var item in Model.TechList) 
                                         { 
                                            <tbody>
                                                <tr>
                                                    <td style="border: 1px solid black;">@item.FirstName @item.LastName </td>
                                                    <td style="border: 1px solid black;">@item.StartDate</td>
                                                
                                                </tr>
                                            </tbody>
                                            }
                                        </table> 
                                     
                                     <br/>
                                        <br/>
                                            <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                             </div>
                                          </div>
                                 </body>
                                   </html>'
 Where NotificationTypeId = 38 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = ' <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                                                              <span>Dear @Model.PersonName,</span>
                                                              <br />
                                                              <br />
                                                              A personal has been added for you at @Model.StartDate<br/>
                                                              Please <a href="@Model.LinkUrl ">click here </a> to review.
                                                           <p>
                                      
                                                         </p>
                                                         <br />
                                                        
                                                 
                                                 <br/>
                                                     <br/>
                                                        <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    
                                                        
                                                        Regards,
                                                             
                                                              <p>
                                                                
                                                                @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                                @Model.AssigneePhone
                                                              </p>
                                                              </div>
                                                          </div>
                                                       </div>
                                              </body>
                                                </html>'
 Where NotificationTypeId = 47 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                 <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                     </style>
                 </head>
                 <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                         <div class="contentEditableContainer contentTextEditable">
                             <div class="contentEditable">
                                 <span>Dear @Model.TechName,</span>
                                 <br>
                                 <br />
                                 <p>
                                 This is to inform you that, your  @Model.jobTypeName  with  Customer  @Model.FullName  on  @Model.StartDate at @Model.Time has been  <b>deleted</b>.
                                <br />
                                <br/>
                                <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPTION</b> - <span>@Model.Description .</span></span>
                             <br/>
                                    Have a great day!
                                     <br/>
                                     <br/>
                                     <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                    </p>
                                 </p>
                             </div>
                         </div>
                 </body>
                 </html>'
 Where NotificationTypeId = 48 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '
       <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
                  <html xmlns="http://www.w3.org/1999/xhtml"> <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /><title>@Model.Base.ApplicationName</title><style type="text/css">body {
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
                      </style>
                  </head>
                  <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                          <div class="contentEditableContainer contentTextEditable">
                              <div class="contentEditable">
                                  <span>Dear @Model.TechName,</span>
                                  <br>
                                  <br />
                                  <p>
                                Esto es para informarle que su trabajo con el cliente   @Model.FullName  el @Model.StartDate a las @Model.Time ha sido <b>eliminado</b>.
                                <br />
                                 <br/>
                                 <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCI?N</b> - <span>@Model.Description .</span></span>
                              <br/>
                                     ?Qu? tenga un lindo d?a!
                                      <br/>
                                      <br/>
                                      <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                  </p>
                              </div>
                          </div>
                  </body>
                  </html>'
 Where NotificationTypeId = 48 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC " -//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml">
             
             <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
             <title>@Model.Base.CompanyName</title>
             <style type="text/css">body {
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
                                               
                                               
                                                       <div class="contentEditableContainer contentTextEditable">
                                                           <div class="contentEditable">
                                                               <span>Dear @Model.CustomerName,</span>
                                                               <br>
                                                               <br>
                                                               
                                                            <p>
                                   Once again, THANK YOU, for the opportunity to assist you with the restoration-and-maintenance of your building surfaces.
                <br><br>
                Your MARBLELIFE proposal is attached for your review and consideration.
             <br><br> 
             Assuming this meets your approval, we can get started and schedule your projects start date either through your salesperson if they are with you now, or with a phone call to our office if approving after our visit.  
                        <br>    <br>
                    </p><div class="@Model.IsSigned">
                     
                        Please review the Invoice attached in the mail and <a href="@Model.Url">Click here</a>  to sign the Invoice. Please use 5 digit code @Model.Code to verify your account.
                    </div>
                    <br><br>
           We look forward to working with you.                         
                                                          <p></p>
                                                         <br>
                                                        
                                                 
                                                     <br>
                                                      <br>
                                                        <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                                           </div>
                                                        </div>
                                               
                                                 </html>'
Where NotificationTypeId = 50 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                                                               <span>Estimada @Model.CustomerName,</span>
                                                               <br />
                                                               <br />
                                                               
                                                            <p>
                                   Una vez m?s, GRACIAS por la oportunidad de ayudar con la restauraci?n y el mantenimiento de las superficies de su edificio.
                <br/><br/>
                Se adjunta la propuesta de MARBLELIFE para su revisi?n y consideraci?n.
             <br/><br/> 
             En el caso de aprobarlo, podemos comenzar y programar la fecha de inicio de sus proyectos, ya sea, a trav?s del vendedor, si est? con usted ahora, o con una llamada telef?nica a nuestra oficina si se aprueba despu?s de nuestra visita.
                        <br/>   <br/>
                    <div class="@Model.IsSigned">
                     
                        Revise la factura adjunta en el correo y<a href=@Model.Url> haga clic aqu?</a>   para firmar la factura. Utilice el c?digo de 4 d?gitos @Model.Code para verificar su cuenta..
                    </div>
                    <br/><br/>
           Esperamos poder trabajar con usted.                          
                                                          </p>
                                                         <br />
                                                        
                                                 
                                                     <br/>
                                                      <br/>
                                                        <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Saludos,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                                                           </div>
                                                        </div>
                                               </body>
                                                 </html>
         '
 Where NotificationTypeId = 50 and LanguageId = 250 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
     <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
            <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
            <title>@Model.Base.CompanyName</title>
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
                     <span>Dear @Model.CustomerName,</span>
                     <br /><br />   
                     <p>
                     Once again, THANK YOU, for the opportunity to assist you with the restoration-and-maintenance of your building surfaces.
                    <br/><br/>
                    @Model.InvoicesName
                    <br/><br/> 
                  <div style="display: @Model.AllInvoicesNotSigned">
                    Assuming this meets your approval, we can consider that our Technician can start his Job for the Invoice(s) signed.
                    </div>
                  <br/><br/>
                       <div style="display: @Model.AllInvoicesSigned">
                            Please review the Invoice attached in the mail.
                        </div>
                        <div style="display: @Model.AllInvoicesNotSigned">
                            Please review the Invoice attached in the mail and <a href=@Model.Url>Click here</a> to sign the unsigned Invoices if you change your mind in the future. Please use 5 digit code @Model.Code to verify your account.
                        </div>
                        <br/>
                        @Model.InvoicesSignedBy
                    <br/><br/>
                    We look forward to working with you.                            
                     </p>
                     <br /><br/>
                     <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                </div>
             </div>
         </body>
     </html>'
Where NotificationTypeId = 51 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>@Model.Base.CompanyName</title>
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
                 <span>Dear @Model.SalesRepName,</span>
                 <br /><br />   
                 <p>
                 Your MARBLELIFE proposal has been signed by the customer. @Model.InvoicesName
                <br/><br/> 
                Assuming this meets your approval, we can consider that our Technician can start his Job for the Invoice(s) signed.
                <br/><br/>
                 <div style="display: @Model.IsSigned">
                     Please review the signed Invoice attached in the mail. You can send the signed invoices from the Application anytime in the future when the customer wishes to start the Job.
                </div>                          
                 </p>
                 <br /><br/>
                 <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
            </div>
         </div>
     </body>
 </html>'
Where NotificationTypeId = 52 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
    <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>@Model.Base.CompanyName</title>
        <style type="text/css">
            body{
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
            .block{
                display:block;
            }
            .none{
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
                   <span>Estimada @Model.CustomerName,</span>
                   <br /><br />
                   <p>
                    Once again, THANK YOU, for the opportunity to assist you with the restoration-and-maintenance of your building surfaces.
                    <br/><br/>
                    Your MARBLELIFE proposal is attached for your review and confirmation of the Job completion.
                    <br/><br/> 
                    Assuming this meets your approval, we can consider that our Technician has completed his Job successfully at your place.
                    <br/><br/>
                    <div style="display: @Model.IsSigned">
                        Please review the Invoice attached in the mail and <a href=@Model.Url>Click here</a> to sign the unsigned Invoices if you change your mind in the future. Please use 5 digit code @Model.Code to verify your account.
                    </div>
                    <br/><br/>
                    We look forward to working with you.                            
                   </p>
                <br /><br/>
                <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
               </div>
           </div>
       </body>
   </html>'
Where NotificationTypeId = 53 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
        <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
            <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
            <title>@Model.Base.CompanyName</title>
            <style type="text/css">
                body{
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
                .block{
                    display:block;
                }
                .none{
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
                    <span>Dear @Model.CustomerName,</span>
                    <br /><br />
                    <p>
                        Once again, THANK YOU, for the opportunity to assist you with the restoration-and-maintenance of your building surfaces.
                        <br/><br/>
                        Your MARBLELIFE proposal is attached for your review and confirmation of the Job completion.
                        <br/><br/> 
                        Assuming this meets your approval, we can consider that our Technician has completed his Job successfully at your place.
                        <br/><br/>
                        <div class="@Model.IsSigned">
                            Please review the Invoice attached in the mail and <a href=@Model.Url>Click here</a> to sign the unsigned Invoices if you change your mind in the future. Please use 5 digit code @Model.Code to verify your account.
                        </div>
                        <br/><br/>
                        We look forward to working with you.                            
                    </p>
                    <br /><br/>
                    <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
                </div>
            </div>
        </body>
    </html>'
Where NotificationTypeId = 54 and LanguageId = 249 ANd Id>0;

update emailtemplate
Set Body = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
   <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>@Model.Base.CompanyName</title>
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
                   <span>Dear @Model.SalesRepName,</span>
                   <br /><br />   
                   <p>
                   Your MARBLELIFE invoice  has been signed upon job completion @Model.InvoicesName
                <br/><br/>
                <br/><br/>
                   
                       Please review the signed Invoice attached in the mail.
                        <br/><br/>      
                          Signature done from @Model.DoneFrom for @Model.CustomerName
                   </p>
                   <br /><br/>
                   <div style="display: @Model.HasCustomSignature">
                    @Model.Signature
                    </div>
                    <div style="display: @Model.NotHasCustomSignature">
                    Regards,
                    <p>
                        @Model.Base.CompanyName<sup>&reg;</sup> <br />
                        @Model.AssigneePhone <br />
                    </p>
                    </div>
            </div>
           </div>
       </body>
   </html>'
Where NotificationTypeId = 55 and LanguageId = 249 ANd Id>0;
