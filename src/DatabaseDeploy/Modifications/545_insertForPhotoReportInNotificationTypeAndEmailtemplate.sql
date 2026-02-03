
insert into notificationtype(Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted)
values('Photo Report Email To Franchisee Owner and Scheduler', 'Photo Report Email To Franchisee Owner and Scheduler', 1, 1, 0);

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsRequired, IsDeleted, languageId, isActive)
Values (581, 61, 'Photo Report Email To Franchisee Owner and Scheduler', 'Photo Report Email To Franchisee Owner and Scheduler', 
'Photo Report Email To Franchisee Owner and Scheduler',
'<!DOCTYPE html
     PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 
         p,
         h1 {
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
 
         div,
         p,
         ul,
         h1 {
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
 
         .block {
             display: block;
         }
 
         .none {
             display: none;
         }
     </style>
 </head>
 
 <body
     style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
     <div class="contentEditableContainer contentTextEditable">
         <div class="contentEditable">
             <span>Good Morning:</span>
             <br>
             <br />
             <p>
                 Attached is the latest SALES and TECH data for next payroll period.
                 <br /><br />
                 Based on your current FRANCHISE DASHBOARD settings your office payroll is set to issue every WEEK, and
                 your next payroll date is expected to be on (15th, 30th, etc) and the data covered in this report covers
                 the period of @Model.StartDate to @Model.EndDate
                 <br /><br />
                 Based on this being the case, attached is a PDF to your PHOTO REPORT which provides 
                 <br /> <br />
                 <ol>
                     <li>The BEFORE-and-AFTER images for each account – allowing a quick review and confirmation that</li>
                     <ul style="list-style-type: none;">
                         <li>(1) Before images are being secured for all sales and (2) paid accounts also have completed BEFORE-and-AFTER image pairings.</li>
                     </ul>
                 </ol>
                 <br /> <br />
                 Regards
                 <br /> <br />
                 MARBLELIFE
             </p>
         </div>
     </div>
 </body>
 </html>', b'1', b'0', 249, b'1');


insert into notificationtype(Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted)
values('Photo Report Email To Sales Rep and Technician', 'Photo Report Email To Sales Rep and Technician', 1, 1, 0);

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsRequired, IsDeleted, languageId, isActive)
Values (582, 62, 'Photo Report Email To Sales Rep and Technician', 'Photo Report Email To Sales Rep and Technician', 
'Photo Report Email To Sales Rep and Technician',
'<!DOCTYPE html
    PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

        p,
        h1 {
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

        div,
        p,
        ul,
        h1 {
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

        .block {
            display: block;
        }

        .none {
            display: none;
        }
    </style>
</head>

<body
    style="background-repeat: repeat; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; -webkit-font-smoothing: antialiased;">
    <div class="contentEditableContainer contentTextEditable">
        <div class="contentEditable">
            <span>Good Morning:</span>
            <br>
            <br />
            <p>
                Attached is the latest SALES and TECH data for next payroll period.
                <br /><br />
                Based on your current FRANCHISE DASHBOARD settings your office payroll is set to issue every WEEK, and
                your next payroll date is expected to be on (15th, 30th, etc) and the data covered in this report covers
                the period of (th to th) (that particular week date would be mentioned)
                <br /><br />
                Based on this being the case, attached is a PDF to your PHOTO REPORT which provides 
                <br /> <br />
                <ol>
                    <li>The BEFORE-and-AFTER images for each account – allowing a quick review and confirmation that</li>
                    <ul style="list-style-type: none;">
                        <li>(1) Before images are being secured for all sales and (2) paid accounts also have completed BEFORE-and-AFTER image pairings.</li>
                    </ul>
                </ol>
                <br /> <br />
                Regards
                <br /> <br />
                MARBLELIFE
            </p>
        </div>
    </div>
</body>
</html>', b'1', b'0', 249, b'1');