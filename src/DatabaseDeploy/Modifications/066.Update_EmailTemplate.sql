UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
				 This is to inform you that, We were asked to reset your @Model.Base.CompanyName account password. 
				<br />
				 Please Click the <b><a href="@Model.PasswordLink" target="_blank">Link</a></b> to set a new password. Please note, this link will expire in 24 hours.
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
</html>'
 WHERE `Id`='1';

 UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
				Welcome to the @Model.Base.ApplicationName System.  
				     <br />                                                 
				Here are your Login credentials -   
				<br>                                              
				<br/>                                                  
				<b>UserName :</b>  @Model.UserName  ,<br /> 
				<b>Password :</b>  @Model.Password                                                                                                                    
				<br/>                                              
				<br> 
				Please click the <a href="@Model.Base.SiteRootUrl" target="_blank" >Link</a> to login.                                                      
				<br> 
				<br/>
				<p>Your ROYALTY REPORTING SYSTEM is where you will report your sales data each reporting period.
				  This data will be used to not just calculate/pay your Adfund and Royalty payment amounts but more importantly will provide the data needed to include your clients in the</p>                 
                  <p>1. Maintenance Mailing System - This system automatically updates your mailing list each time you report. </p>
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
</html>'
 WHERE `Id`='2';




UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
                                <td>$ @item.AdFund</td>
                                <td>$ @item.Royalty</td>
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
 WHERE `Id`='3';

  UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
                                <td>$ @item.AdFund</td>
                                <td>$ @item.Royalty</td>
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
 WHERE `Id`='4';

 UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
                                <th style="border: 1px solid black;">Satrt Date</th>
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

 UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
 WHERE `Id`='6';

 UPDATE `EmailTemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
 WHERE `Id`='7';


UPDATE `makalu`.`emailtemplate` SET `Body`=
'<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">
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
                   This is for your kind information, that as a result of delay in upload of SalesData, Late Fee has been charged. Please see the invoice listed below:
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

                   <b>Kindly upload the files as earliest as possible, to avoid Late Fee charges..</b>
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
</html>'
 WHERE `Id`='8';
