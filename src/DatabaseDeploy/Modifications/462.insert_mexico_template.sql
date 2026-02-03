INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('510', '1', 'USER - Forget Password','Forget Password','USER - Login Credentiall | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				 Le escribimos para informarle que se nos solicitó restablecer la contraseña de su cuenta de @Model.Base.CompanyName account password. 
 				<br />
 				 Haga clic en el <b><a href="@Model.PasswordLink" target="_blank">enlace </a></b> para establecer una nueva contraseña. Tenga en cuenta que este enlace caducará en 24 horas.
                     <br>
                     <br />                  
                     Saludos,
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
 </html>');
 
 
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('520', '2', 'USER - Login Credential','Login Credential','USER - Login Credentiall | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                 <span>Estimada @Model.FullName,</span>
                 <br>
                 <br />
                 <p>
 				Le damos la bienvenida al @Model.Base.ApplicationName System.  
 				     <br />                                                 
 				Estas son sus credenciales para iniciar sesión -   
 				<br>                                              
 				<br/>                                                  
 				<b>Usuario :</b>  @Model.UserName  ,<br /> 
 				<b>Contraseña :</b>  @Model.Password                                                                                                                    
 				<br/>                                              
 				<br> 
 				Por favor, haga clic en el  <a href="@Model.Base.SiteRootUrl" target="_blank" >enlace </a> para iniciar sesión..                                                      
 				<br> 
 				<br/>
 				<p>Su SISTEMA INFORMES DE REGALÍAS es donde reportará sus datos de ventas en cada período de reporte.
 				   Estos datos se utilizarán no solo para calcular/liquidar sus pagos de Adfund y regalías, y de mayor importancia, proporcionarán los datos necesarios para incluir a sus clientes en el </p>                 
                   <p>1. Sistema de correo de mantenimiento - este sistema actualiza automáticamente su lista de correo cada vez que presente su reporte. </p>
 				  				  <br> 
 				<br/>
 				   Saludos,
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
 </html>');
 
 
 
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('530', '3', 'ACCOUNTING - Invoice Notification','Invoice Notification','ACCOUNTING - Invoice Notification | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				TGracias por cargar sus DATOS AL INFORME DE REGALÍAS DE MARBLELIFE para el período del @Model.StartDate to @Model.EndDate.<br>
 				A continuación encontrará una copia de los montos de sus facturas de Adfund/Regalías.
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
                                 <td>$ @item.AdFund</td>
                                 <td>$ @item.Royalty</td>
                                 <td style="border: 1px solid black;">$ @item.TotalPayment</td>
                             </tr>
                         </tbody>
 						}}
                     </table>
 
                     <br />
                     <br />
 
                     <b>Asegúrese de que los pagos se realicen antes del @Model.DueDate para evitar cargos por demora o intereses.</b>
                     <br /><br />
                     Gracias una vez más por tomarse el tiempo de cargar sus datos y mantener el pago de su cuenta al día.
 
                     <br>
                     <br />
                     Saludos,
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
 </html>');
 
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('540', '4', 'ACCOUNTING - Payment Reminder','Payment Reminder','ACCOUNTING - Payment Reminder | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				Esto es para informarle que tiene los siguientes pagos pendientes.
 				<br />
 				Por favor, vea los detalles a continuación:
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
                                 <td>$ @item.AdFund</td>
                                 <td>$ @item.Royalty</td>
                                 <td style="border: 1px solid black;">$ @item.Amount</td>
                             </tr>
                         </tbody>
 						}}
                     </table>
 					 <br />
                     <br />
 
                     <b>Tenga en cuenta que, en caso de que los pagos no se realicen en la fecha de vencimiento, se aplicará un cargo por pago atrasado. Falta de pago adicional resultará en un monto de interés que se aplicará diariamente.</b>
                     <br /><br />
                     Por favor haga el pago antes de la fecha de vencimiento para evitar estos cargos.
                     <br /> <br />
                     Saludos,
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
 </html>');
 
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('550', '5', 'ACCOUNTING - Sales Data Upload Reminder','Sales Data Upload Reminder','ACCOUNTING - Sales Data Upload Reminder | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                      Esto es para recordarle que no hemos recibido datos de ventas para este período:
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
                      
                      Saludos,
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
  </html>');
 
 
  INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('560', '6', 'ACCOUNTING - Payment Late Fee','LateFee  For Payment','ACCOUNTING - Payment Late Fee | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                    Le escribimos para informarle, que como resultado de la falta de pago del número de factura:- @Model.InvoiceId, se ha cobrado un Cargo por Pago atrasado. Consulte la factura que se detalla a continuación:
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
 
                     <b>Tenga en cuenta que, en caso de que los pagos no se realicen en la fecha de vencimiento, se aplicará un cargo por pago atrasado. Falta de pago adicional resultará en un monto de interés que se aplicará diariamente.</b>
                     <br /><br />
                     Por favor haga el pago antes de la fecha de vencimiento para evitar estos cargos.
 
                     <br>
                     <br />
                     
                     Saludos,
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
 </html>');
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('570', '7', 'ACCOUNTING - Payment Confirmation','Payment Confirmation','ACCOUNTING - Payment Confirmation| @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				Esto es para confirmar que hemos recibido el pago de la cantidad de $ 85.54, para el número de factura - @Model.InvoiceId.
 				<br />
 				Consulte los detalles de la factura a continuación:
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
                     Saludos,
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
 </html>');
 
 
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('58', '8', 'ACCOUNTING - Sales Data Upload Latefee','LateFee  For SalesDataUpload','ACCOUNTING - Sales Data Upload Latefee | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                    Esto es para informarle, que como resultado de la demora en la carga de datos de venta, se ha cobrado un cargo por pago atrasado. Consulte la factura que se detalla a continuación:
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
                     
                    Saludos,
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
 </html>');
 
 
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('59', '9', 'ACCOUNTING - Sales Data Upload Latefee','LateFee  For SalesDataUpload','ACCOUNTING - Sales Data Upload Latefee | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                    Esto es para informarle, que como resultado de la demora en la carga de datos de venta, se ha cobrado un cargo por pago atrasado. Consulte la factura que se detalla a continuación:
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
                     
                    Saludos,
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
 </html>');
 
 INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('60', '15', 'ACCOUNTING - Monthly Sales Upload Notification','Monthly Sales Upload Notification','ACCOUNTING - Monthly Sales Upload Notification','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                       <font size="3" color="#454545">
     					 Estimada  @Model.Base.OwnerName,<br /><br />
   							Adjunto encontrará la lista de franquiciados que no han cargado sus Datos de Venta a tiempo.<br /><br />
   							 Saludos,<br /><br />
    						
     					@Model.Base.CompanyName <br>
     					@Model.Base.Phone <br>
     				</font>             
      					<br>
                         <br />	
                       </p> 				  
                   </div>
               </div>
       </body>
       </html>');
	   
	   
	   INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('61', '16', 'Annual Upload Failed','Annual Upload Failed','Annual Upload Failed  | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
  				Su carga de ventas anuales para el año @Model.Year  ha fallado. Verifique los registros y cargue el archivo correcto.
  				<br>                                              
  				<br/>                                                  
 				Saludos,
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
  </html>');
  
  
  INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('62', '17', 'Annual Upload Success','Annual Upload Success','Annual Upload Success | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				 Sus ventas anuales para el año @Model.Year se cargaron con éxito. Le notificaremos una vez que nuestro equipo las revise.
  				<br>                                              
  				<br/>                                                  
 				Saludos,
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
  </html>');
  
  
    INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('63', '18', 'Annual Upload Review','Annual Upload Review','Annual Upload Review | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                  <span>Estimada @Model.AdminName,</span>
                  <br>
                  <br />
                  <p>
 				 @Model.Franchisee ha subido sus ventas anuales para el año @Model.Year, revise amablemente la carga y tome las medidas necesarias.
 				 <br>                                              
  				<br/>                                                  
 				Saludos,
                      <br /><br />
  					<p>
                     @Model.Base.CompanyName <br>
      				@Model.Base.Phone <br>
  					</p>
                  </p>
              </div>
          </div>
  </body>
  </html>');
  
  
     INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('64', '19', 'Annual Upload Accepted','Annual Upload Accepted','Annual Upload Accepted | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
  				Nuestro equipo ha revisado y aprobado su carga anual para el año @Model.Year.  
  				<br>                                              
  				<br/>                                                  
 				Saludos,
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
  </html>'); 
  
  
     INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('65', '21', 'USER - Login Credential','Login Credential','USER - Login Credential | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
  				Bienvenido al Sistema de @Model.Base.SchedulingAppliation System.  
  				     <br />                                                 
  				Aquí están sus credenciales de inicio de sesión: 
  				<br>                                              
  				<br/>                                                  
  				<b>Usuario :</b>  @Model.UserName  ,<br /> 
  				<b>Contraseña :</b>  @Model.Password                                                                                                                    
  				<br/>                                              
  				<br> 
  				Para ingresar y ver su calendario visitará  <a href="@Model.Base.SiteRootUrl" target="_blank" >liga</a> e ingresará su combinación USUARIO/PW..                                                      
  				<br> 
  				<br/>
  				<p>Una vez ahi podrá ver su calendario diario de presupuestos y trabajos.
  				   Por favor tome un minuto y configure cualquier día PERSONAL que tenga para asegurarse de que este tiempo se reserve, ya sea por vacaciones, atención médica u otro motivo de personal.</p>   
                    <br/>				  
                    <p>Gracias por ser parte del equipo. </p>
  				  				  <br> 
  				<br/>
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
  </html>');   
  
  
  
     INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('66', '22', 'USER - Document Upload','Document Upload','USER - Document Upload | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                   <span>Estimada @Model.FullName,</span>
                   <br>
                   <br />
                   <p>
 				     <b>@Model.UploadedBy</b> (<i>@Model.Email/@Model.Role</i>) de <b>@Model.Franchisee</b> ha subido un documento @Model.DocName.
                      Se marca como importante.			 
                      <br>
                       <br />                    
                      Por favor revíselo.
                       <br>
                       <br />
                       
                       Saludos,
                       <br /><br />
   					<p>
   					@Model.Base.CompanyName <br>
   					@Model.Base.Phone <br>
   					</p>
                   </p>
               </div>
           </div>
   </body>
   </html>');   
   
   

     INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('67', '23', 'Document Expiry','Document Expiry','Owner - Document Expiry | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				     Documento @Model.DocName expirará el @Model.ExpiryDate. Por favor, descárguelo si lo necesita.				 
                      <br>
                       <br />                    
                                            
                       Saludos,
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
   </html>');
   
   
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('68', '24', 'Owner - Document Upload to Franchisee','Document Upload','Owner - Document Upload to Franchisee | @Model.Franchisee','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 				     <b>@Model.UploadedBy</b> (<i>@Model.Email</i>) ha subido un documento ML de NE FL Addendum to @Model.DocName.
                      Esta marcado como importante.				 
                      <br>
                       <br />                    
                       Por favor revíselo.
                       <br>
                       <br />
                       
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
   </html>');  
   
   

INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('69', '25', 'CUSTOMER - Job Scheduled Confirmation','Your Upcoming Job','CUSTOMER -  @Model.jobType Scheduled Confirmation','250', '
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
                    Esto es para confirmar su cita con MARBLELIFE <sup>&reg;</sup> el @Model.StartDate el @Model.Time con el Representante @Model.techNames Esperamos verte y ayudarte con tu proyecto. <br/><br/>¡Qué tengas un lindo día!
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
                    Saludos,
                                     
                                       <p>
                                         @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                         @Model.AssigneePhone <br />
                         </p>
                                       
                                   </div>
                                </div>
                        </body>
                        </html>');    
						
						
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('70', '26', 'TECH - New Job Assignment','New Job Schedule for Tech','@Model.jobType New: @Model.StartDate  , @Model.FullName, @Model.Address, @Model.FranchiseeName','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                        .block{
                        display:block;
                        }
                        .none{
                        display:none;
                        }
                    </style>
                </head>
                <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
                        <div class="contentEditableContainer contentTextEditable">
                            <div class="contentEditable">
                                <span>Estimado @Model.TechName,</span>
                                <br/>
                                <br/>
                                <p>
                				 Ha sido programado para @Model.jobTypeName con los detalles a continuación::
            					 <p>
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
                                  <br/>
                             Se ha incluido el número de teléfono del cliente para que pueda confirmar que está en camino la mañana del trabajo, y en caso de que se retrase en el camino y necesite alertar al cliente, @Model.PhoneNumber.<br/><br/>Please <a href="@Model.LinkUrl"> Haga clic</a> Haga clic aquí para ver el  @Model.jobTypeName .
                           <br/><br/>
                            <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b>-<span>@Model.Description</span></span>
                            </p>
               				<br /><br />
                            
            					¡Que tenga un buen día!
                                    <br/>
            						<br/>            
                                    Saludos,
                					<p>
                                  @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                  @Model.AssigneePhone <br />
                					</p>
                                </p>
                            </div>
                        </div>
                </body>
                </html>');						
				
				
				
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('71', '27', 'TECH - Job Cancelation','Job Cancelled for Tech','@Model.jobType Cancelled: @Model.StartDate , @Model.FullName, @Model.Address, @Model.FranchiseeName','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                                <span>Estimado @Model.TechName,</span>
                                <br>
                                <br />
                                <p>
                				 TEsto es para informarle que su  @Model.jobTypeName   piso del contrato  @Model.FullName  del @Model.StartDate a las @Model.Time ha sido <b>cancelado</b>.
               				<br />
                               <br/>
                               <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b> - <span>@Model.Description .</span></span>
                            <br/>
                                   ¡Que tengas un buen día!
                                    <br/>
                                    <br/>
                                    Saludos,
                                   
                					<p>
                                  @Model.Base.CompanyName <sup>&reg;</sup><br />
                                  @Model.AssigneePhone <br />
                					</p>
                                </p>
                            </div>
                        </div>
                </body>
                </html>');										
				
				
				
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('72', '28', 'TECH - Job Update','Job Updation for Tech','@Model.jobType Rescheduled to  @Model.StartDate  for @Model.FullName: @Model.FranchiseeName','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                                 <span>Estimado @Model.TechName,</span>
                                 <br>
                                 <br />
                                 <p>
                              
              Los detalles sobre su trabajo modificado @Model.jobTypeName muestran a continuación::
               <p>
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
                                       <br/>
              Se ha incluido el número de teléfono del cliente para que pueda confirmar que está en camino después de su estimación anterior para ese día, o que se ha retrasado y necesita alertar al cliente.   @Model.PhoneNumber
                            </p>
                             <br/><br/>
                                 <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b> - <span>@Model.Description .</span></span><br/><br/> Por favor <a href="@Model.LinkUrl"> haga clic aquí</a> para ver el @Model.jobTypeName .<br/> ¡Que tengas un buen día!
                                     <br/>
                                     <br/>
                                     Saludos,
                                     <br />
                 					<p>
                                
                                   @Model.Base.CompanyName <br />
                                   @Model.AssigneePhone <br />
                 					</p>
                                 </p>
                             </div>
                         </div>
                 </body>
                 </html>');	



INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('73', '32', 'TECH - Urgent Job Update','Urgent Job Change','URGENT NEW @Model.jobType SCHEDULED FOR @Model.dateType  @Model.StartDate  with @Model.FullName at @Model.Address:  @Model.FranchiseeName','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                               Tenemos un DE ÚLTIMA HORA  @Model.jobTypeName que se ha agregado a su horario con los detalles a continuación:<br/>
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
                               <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b> - <span>@Model.Description</span></span>
                              <span> <br/><br/>Please <a href="@Model.LinkUrl"> Haga clic aquí </a> para ver el @Model.jobTypeName .<span>
                               <br />    <br/>              
                                   ¡Qué tenga un lindo día!</br>
                               ¡Buena suerte!
                               </p>
                                <br/>
                               Saludos,
                           </div>
                           <p>
                                      @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                      @Model.AssigneePhone <br />
                           </p>
                   
                       </div>
                   
                   </body>
                   </html>');				 
				   
				   
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('74', '34', 'Before After Images','Before After Images','Before-After Images Job/Estimate','250', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                               <span>Estimada @Model.CustomerName,</span>
                               <br>
                               <br />
                               <p>
              				
               				Gracias una vez más por la oportunidad de ayudarlo a mejorar su edificio. 
    
    Adjunto encontrará UN ANTES y DESPUÉS de su proyecto para sus registros... y para compartir con amigos y familiares. 
 Esperamos trabajar con usted nuevamente en su próximo proyecto.
    
    
    
                              <br />
              				<br/>
                                   Saludos,
                                   <br />
               					<p>                    
               					@Model.Base.CompanyName <br>
               					</p>
                               </p>
                           </div>
                       </div>
               </body>
               </html>');					   
			   

INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('76', '37', 'CUSTOMER-New Job Scheduled','New Job Created','CUSTOMER-New @Model.jobType Scheduled','250', '
       
          
        
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
                                              <span>Estimada @Model.FullName,</span>
                                              <br />
                                              <br />
                                              
                                           <p>
                           Su PROYECTO MARBLELIFE ha sido programado con el técnico/representante de ventas de MARBLELIFE ® @Model.techDesignation  @Model.techNames  Esperamos verle pronto y ayudarle con su proyecto. <br/><br/>¡Qué tenga un lindo día!
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
                           Regards,
                                             
                                              <p>
                                                @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                @Model.AssigneePhone <br />
                                </p>
                                              
                                          </div>
                                       </div>
                              </body>
                                </html>
          
         ');				   
	 
	 
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('77', '38', 'CUSTOMER-Job Schedule Update','Updation in Your Job','CUSTOMER- @Model.jobType Schedule Update','250', '
           
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
                            Ha habido una actualización en su PROYECTO MARBLELIFE programada con los técnicos/representantes de ventas de MARBLELIFE @Model.techDesignation @Model.techNames . <br/> Esperamos poder ayudarle con su proyecto.<span> <br/><br/>Please <a href="@Model.LinkUrl"> Haga clic aquí </a> para ver el @Model.jobType .<span> <br/><br/>¡Qué tenga un lindo día!
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
                            				Saludos,
                                               
                                                <p>
                                                  @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                  @Model.AssigneePhone <br />
                                				  </p>
                                                
                                            </div>
                                         </div>
                                </body>
                                  </html>
          
     ');	 

    INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('78', '31', 'SALES/TECH - Job-Estimate REASSIGNED','ReAssigned Job/Estimate','@Model.jobType Re-assigned: @Model.StartDate : @Model.FullName, @Model.Address: @Model.FranchiseeName','250', '
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
inicialmente un mensaje que indicaba que otra persona vendría, pero acaba de recibir un mensaje que indica que su trabajo se ha reasignado a usted, con los detalles a
 continuación:
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
                               <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b> - <span>@Model.Description</span></span>
                               <span> <br/><br/>Please <a href="@Model.linkUrl"> Haga clic aquí</a> para ver el trabajo. .<span>
                                    <br />    <br/>              
               					¡Que tengas una gran visita!
                                      <br>
                                      <br />                  
                                      Saludos,
                  					<p>
                                    @Model.Base.CompanyName<sup>&reg;<sup><br />
                                    @Model.AssigneePhone <br />
                  					</p>
                                  </p>
                              </div>
                           </div>
                  </body>
                  </html>');

    INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('79', '34', 'Before After Images','Before After Images','Before-After Images Job/Estimate','250', '
      <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                               <span>Estimada @Model.CustomerName,</span>
                               <br>
                               <br />
                               <p>
              				
               				Gracias una vez más por la oportunidad de ayudarlo a mejorar su edificio. 
    
    Adjunto encontrará UN ANTES y DESPUÉS de su proyecto para sus registros... y para compartir con amigos y familiares. 
    
    Esperamos trabajar con usted nuevamente en su próximo proyecto.
    
    
                              <br />
              				<br/>
                                   Saludos,
                                   <br />
               					<p>                    
               					@Model.Base.CompanyName <br>
               					</p>
                               </p>
                           </div>
                       </div>
               </body>
               </html>');

INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('81', '48', 'Job/Estimate Deletion','Job/Estimate Deletion','@Model.jobType  Deleted | @Model.FranchiseeName','250', '
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
                                <span class="@Model.IsDisplayVisible"><b>@Model.jobTypeName DESCRIPCIÓN</b> - <span>@Model.Description .</span></span>
                             <br/>
                                    ¡Qué tenga un lindo día!
                                     <br/>
                                     <br/>
                                     Saludos,
                                    
                 					<p>
                                   @Model.Base.CompanyName <sup>&reg;</sup><br />
                                   @Model.AssigneePhone <br />
                 					</p>
                                 </p>
                             </div>
                         </div>
                 </body>
                 </html>');


INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('82', '49', 'RPID Mail To Franchisee Admin','RPID Mail To Franchisee Admin','RPID Missing for @Model.FranchiseeName','250', '
      
              
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
                                                   <span>Estimado,</span>
                                                   <br />
                                                   <br />
                                                   
                                                <p>
                          No se ha encontrado ningún RPID para el franquiciado @Model.FranchiseeName. Por favor agregue el RPID adecuado en el Panel del franquiciado.
        						
                                              </p>
             								 <br />
             								
               						 
                						 <br/>
                                          <br/>
                               				Saludos,
                                                  
                                                   <p>
                                                     @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                     @Model.AssigneePhone <br />
                                   				  </p>
                                                   
                                               </div>
                                            </div>
                                   </body>
                                     </html>
        ');

INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('83', '50', 'Invoice To Customer','Invoice To Customer','Invoice To Customer','250', '
    
                         
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
                                                              <span>Estimada @Model.CustomerName,</span>
                                                              <br />
                                                              <br />
                                                              
                                                           <p>
                                  Una vez más, GRACIAS por la oportunidad de ayudar con la restauración y el mantenimiento de las superficies de su edificio.
               <br/><br/>
               Se adjunta la propuesta de MARBLELIFE para su revisión y consideración.
          	 <br/><br/> 
          	 En el caso de aprobarlo, podemos comenzar y programar la fecha de inicio de sus proyectos, ya sea, a través del vendedor, si está con usted ahora, o con una llamada telefónica a nuestra oficina si se aprueba después de nuestra visita.
                   	<br/>	<br/>
                   <div class="@Model.IsSigned">
                    
                       Revise la factura adjunta en el correo y<a href=@Model.Url> haga clic aquí</a>   para firmar la factura. Utilice el código de 4 dígitos @Model.Code para verificar su cuenta..
         			</div>
         			<br/><br/>
          Esperamos poder trabajar con usted.							
                                                         </p>
                        								 <br />
                        								
                          						 
                           						 <br/>
                                                     <br/>
                                          				Saludos,
                                                             
                                                              <p>
                                                                @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                                                @Model.AssigneePhone <br />
                                              				  </p>
                                                              
                                                          </div>
                                                       </div>
                                              </body>
                                                </html>
        ');


        	 
INSERT INTO `makalu`.`emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`,`LanguageId`, `Body`) VALUES ('92', '45', 'Renewable Mail To Franchisee','Renewable Mail To Franchisee','Renewable Mail to Franchissee | @Model.FranchiseeName','250', '
                                                                 
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
                                                        <span>Estimado @Model.PersonName,</span>
                                                        <br />
                                                        <br />
                                                        
                                                     <p>
                               Ha pasado otro mes y estamos otro mes más cerca de la fecha de renovación de su acuerdo. Tómese un momento y comuníquese con MARBLELIFE para hacer los arreglos para su renovación. Esperamos trabajar con usted durante otros 10 años.<br>
                                               <br/>
                                    				Saludos,
                                                       
                                                        <p>
                                                          Alan Mayr <br />
                                                          @Model.Base.CompanyName<sup>&reg;</sup> <br />
                                        				  </p>
                                                        
                                                    </div>
                                                 </div>
                                        </body>
                                          </html>
     ');	 

     