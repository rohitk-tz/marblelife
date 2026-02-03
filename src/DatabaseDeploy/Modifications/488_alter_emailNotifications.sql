Alter table customersignature
Add column `InvoiceNumber` bigint(20) NULL default Null;

Insert into notificationtype (Id, Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted) Values (51, 'Signed Invoice To Customer', 'Signed Invoice To Customer', b'1', b'1', b'0');

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsDeleted, languageId, isActive)
Values (571,	51,	'Signed Invoice To Customer', 'Signed Invoice To Customer', 'Signed Invoice To Customer','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
					Una vez más, GRACIAS por la oportunidad de ayudar con la restauración y el mantenimiento de las superficies de su edificio.
					<br/><br/>
					Se adjunta la propuesta de MARBLELIFE para su revisión y consideración.
					<br/><br/> 
					En el caso de aprobarlo, podemos comenzar y programar la fecha de inicio de sus proyectos, ya sea, a través del vendedor, si está con usted ahora, o con una llamada telefónica a nuestra oficina si se aprueba después de nuestra visita.
					<br/>br/>
					<div class="@Model.IsSigned">
						Revise la factura adjunta en el correo y<a href=@Model.Url> haga clic aquí</a>   para firmar la factura. Utilice el código de 4 dígitos @Model.Code para verificar su cuenta..
					</div>
					<br/><br/>
					Esperamos poder trabajar con usted.							
                </p>
				<br /><br/><br/>Saludos,
                <p>
                    @Model.Base.CompanyName<sup>&reg;</sup> <br />
                    @Model.AssigneePhone <br />
                </p>
            </div>
        </div>
    </bo
</html>', b'0', 250, b'1');
 
UPDATE `makalu`.`emailtemplate` SET `Body` = '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
  				Assuming this meets your approval, we can consider that our Technician can start his Job for the Invoice(s) signed.
  				<br/><br/>
                    <div class="@Model.AllInvoicesSigned">
  						Please review the Invoice attached in the mail.
  					</div>
 					<div class ="@Model.AllInvoicesNotSigned">
  						Please review the Invoice attached in the mail and <a href=@Model.Url>Click here</a> to sign the unsigned Invoices if you change your mind in the future. Please use 4 digit code @Model.Code to verify your account.
  					</div>
  					<br/>
  					@Model.InvoicesSignedBy
           		<br/><br/>
  				We look forward to working with you.							
                  </p>
                  <br /><br/><br/>
  				Regards,
                  <p>
                    @Model.Base.CompanyName<sup>&reg;</sup> <br />
                    @Model.AssigneePhone <br />
                  </p>
  			</div>
          </div>
      </body>
  </html>' WHERE (`Id` = '571');