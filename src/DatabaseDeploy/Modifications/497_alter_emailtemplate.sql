insert into notificationtype (Id, Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted) Values (57, 'Web Leads Report: Data not received', 'Web Leads Report: Data not received', b'1', b'1', b'0');

insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsDeleted, LanguageId, IsActive) values (577, 57, 'Web Leads Report: Data not received', 'Web Leads Report: Data not received', 'Web Leads Report: Data not received', '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
				<span>Dear Team,</span>
				<br /><br />
				<p>
				No new data has been received from Web Leads from the API on @Model.Date.
				</p>
				<br /><br/><br/>Regards,
				<p>
					@Model.Base.CompanyName<sup>&reg;</sup> <br />
				</p>
			</div>
		</div>
	</body>
</html>', b'0', 249, b'1');