
insert into notificationtype(Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted)
values('Send Link To Franchisee Owner And Scheduler For Payroll Report', 'Send Link To Franchisee Owner And Scheduler For Payroll Report', 1, 1, 0);

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsRequired, IsDeleted, languageId, isActive)
Values (579, 59, 'Send Link To Franchisee Owner And Scheduler For Payroll Report', 'Send Link To Franchisee Owner And Scheduler For Payroll Report', 
'Send Link To Franchisee Owner And Scheduler For Payroll Report',
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
            width: 800px;
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
                Attached is the latest SALES/TECH data for next payroll period.
                <br /><br />
                Based on your current FRANCHISE DASHBOARD settings your office payroll is set to issue every (WEEK,
                MONTH, BI-WEEK, BI-MONTH – MOST WILL BE BI-MONTH), and your next payroll date is expected to be on
                (15th, 30th, etc) and the data covered in this report covers the period of (@Model.NotificationForPayrollReportViewModels.FromDate th to @Model.NotificationForPayrollReportViewModels.ToDate th).
                <br /><br />
                <a href="@Model.NotificationForPayrollReportViewModels.URL">@Model.NotificationForPayrollReportViewModels.URL</a>
                <br /><br />
                Based on this being the case, attached is a link to your PAYROLL REPORT which provides
                <br />
                <br />
                <b>Sales Data for each sales person including</b>
            <ol>
                <li>their SALES during the period and the ACCRUAL amount SOLD</li>
                <li>their SALES COLLECTED during the period (Cash – from Royalty reporting system). Sales roles includes
                    collecting deposits and assisting in collecting final payment if required.</li>
                <li>The BEFORE-and-AFTER images for each account – allowing a quick review and confirmation that (1)
                    Before images are being secured for all sales and (2) paid accounts also have completed
                    BEFORE-and-AFTER image pairings.</li>
            </ol>
            <br />
            <b>Tech Date for each technician including:</b>
            <ol>
                <li>The projects the technician was assigned to along with the PROJECT VALUE</li>
                <li>The HOURLY rate and PRODUCTIVITY rate for the tech that appears in their USER file for ease of
                    reference as you prepare their payroll amounts to facilitate the payroll researching effort.</li>
                <li>Before-and-after images for each project so one can CONFIRM that AFTER images are being collected,
                    and address any missing images on the basis that this may impact the techs final pay, as they are
                    paid to complete the project inclusive of paperwork (photo uploads) and securing of final payment or
                    submission-of-a-trip ticket.</li>
            </ol>
            <br />
            As one reviews this data, you can also
            <br />
            <ol>
                <li>Select the BEFORE-and-AFTER image that best reflects the clients project – and should appear on
                    their
                    MAINTENANCE CARD. We are planning to move to digital maintenance cards that would show the before
                    and after
                    of THEIR project so that they can relate to how bad it was and how good it looked afterwards, with
                    the
                    expectation that they can better assess the need for a REFRESH SERVICE when comparing to the AFTER,
                    and have
                    the BEFORE reminding them how bad it could get. If an image is not selected they will see a standard
                    card
                    with a non-related before-and-after image.
                    <br /><br />
                    Select any pairs that you would like to have automatically added to your LOCAL WEB PAGE Gallery. The
                    system will automatically place these in the correct GALLERY based on the selections made on the
                    images (SERVICE, Surface type, Room type). This will allow you to build out your local web page more
                    rapidly and have it reflect your local office work, adding additional credibility for your local
                    office. These images will push the initial national images deeper into the gallery to favor local
                    work over national work.
                </li>
            </ol>
            The intent of this report is to assist in delivering to you the relevant data you need to quickly assemble
            your payroll reports for your teams. This should facilitate the calculation of PRODUCTIVITY or COMMISSIONS.
            Hourly data would still need to be pulled from your local HOUR reporting process.
            <br /> <br />
            Your SALES and TECHS will also get a copy of their portion of the report, so that they know what is being
            shown AND they can facilitate the upload of any missing images to complete their package automatically.
            <br /> <br />
            Please note
            <ol>
                <li>In the case where you may have multiple techs working on the same project the project will appear on
                    both techs reports, however the SPLIT of the project would still need to be determined by management
                    at this time.</li>
            </ol>
            Let us know if you have an idea on how we can boost the value of this report to you as you prepare your payroll and manage your office.
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
values('Send Link To Sales Rep And Technician For Payroll Report', 'Send Link To Sales Rep And Technician For Payroll Report', 1, 1, 0);

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsRequired, IsDeleted, languageId, isActive)
Values (580, 60, 'Send Link To Sales Rep And Technician For Payroll Report', 'Send Link To Sales Rep And Technician For Payroll Report', 
'Send Link To Sales Rep And Technician For Payroll Report',
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
                Attached is your latest SALES/TECH data for next payroll period as assembled by the
                MARBLELIFE-INFORMATION-MANAGEMENT-SYSTEM.
                <br /><br />
                <a href="@Model.NotificationForPayrollReportViewModels.URL">@Model.NotificationForPayrollReportViewModels.URL</a>
                <br /><br />
                Your office payroll is set to issue every (WEEK, MONTH, BI-WEEK, BI-MONTH – MOST WILL BE BI-MONTH), and
                your next payroll date is expected to be on (15th, 30th, etc). This report covers data from @Model.NotificationForPayrollReportViewModels.FromDate Th to
                @Model.NotificationForPayrollReportViewModels.ToDate th, which will be used for this next payroll period.
                <br /> <br />
                Please review the data for accuracy, and report any issues to your MANAGER.
                <br /> <br />
                This is also your opportunity to see if you are missing any PHOTO uploads as this may impact the status
                of your project’s COMPLETION which includes JOB, PHOTO SUBMISSIONS, and COLLECTION OF PAYMENT.
                <br /> <br />
                Be sure to ensure that your project includes your BEFORE and AFTER images for the project. This data is
                used by the business to promote the business to new clients, and is also used to confirm completion of
                the project. This is also intended to enable you to confirm that all projects you worked on are
                reflected in the report, so any discrepancies can be addressed prior to payroll finalization and
                issuance.
                <br /> <br />
                We appreciate your efforts as always.
                <br /> <br />
                Regards
                <br /> <br />
                MARBLELIFE
            </p>
        </div>
    </div>
</body>
</html>', b'1', b'0', 249, b'1');