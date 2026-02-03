INSERT INTO `notificationtype` (`Id`, `Title`, `Description`, `IsQueuingEnabled`, `IsServiceEnabled`, `IsDeleted`) 
VALUES ('14', 'Monthly Mailchimp Report', 'Monthly Mailchimp Report', 1, 1, 0);

INSERT INTO `emailtemplate` (`Id`, `NotificationTypeId`, `Title`, `Description`, `Subject`, `Body`) 
VALUES ('14', '14', 'Monthly Mailchimp Report', 'Monthly Mailchimp Report', 'Monthly Mailchimp Report', 
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
                padding: 5px;
                margin: 2px;
                text-align: left;
            }
    
            table tbody tr td {
                padding: 5px;
                margin: 2px;
                text-align: left;
            }
        </style>
    </head>
    <body style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
            <div class="contentEditableContainer contentTextEditable">
                <div class="contentEditable">
                    <font size="3" color="#454545"><span>Dear @Model.FullName,</span>
                    <br>
                    <br />                   
   				  <p>
				  Please find the list of emails synced with <b>MailChimp API</b> in the duration of <b>@Model.Startdate - @Model.EndDate</b> in the attachment below.
 				  <br />
    				<br />
                    </p>
 				   Regards,
                        <br /><br />
    					<p>
                     @Model.Base.CompanyName <br>
    					@Model.Base.Phone <br>
 					</font>  
                </div>
            </div>
    </body>
    </html>'
);
