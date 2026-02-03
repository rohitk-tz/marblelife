
update emailtemplate
Set Title = 'MarbleLife Proposal', Description = 'MarbleLife Proposal', Subject = 'MarbleLife Proposal'
where (Id = 50 and NotificationTypeId = 50);

update emailtemplate
Set Title = 'MarbleLife Proposal', Description = 'MarbleLife Proposal', Subject = 'MarbleLife Proposal'
where (Id = 83 and NotificationTypeId = 50);

update emailtemplate
Set Body = '<!DOCTYPE html
    PUBLIC " -//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
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

    .btn-confirm {
        background-color: #26327e !important;
        margin-left: 30% !important;
        color: white !important;
        border: none !important;
        width: 40% !important;
        margin-top: 5% !important;
        height: 42px !important;
    }

    .btn-cancel {
        background-color: #26327e !important;
        margin-left: 30% !important;
        color: white !important;
        border: none !important;
        width: 38% !important;
        margin-top: 5% !important;
        height: 42px !important;
    }
</style>

<div class="contentEditableContainer contentTextEditable">
    <div class="contentEditable">
        <span>Dear @Model.CustomerName,</span>
        <br>
        <br>
        <p>
            Once again, THANK YOU, for the opportunity to assist you with your project.
            <br><br>
            Your MARBLELIFE proposal is attached for your review and consideration.
            <br><br>
            You will need this unique 5 digit code @Model.Code to access your proposal.
            <br> <br>
            Should you have any questions regarding your project or proposal you can reach me at the number below or contact our office at @Model.OfficeNumber
            <br> <br>
        </p>
        <div class="@Model.IsSigned">
            When ready to APPROVE, <a href="@Model.Url">Click here</a> to sign the proposal.
        </div>
        <br>Then call our office at @Model.OfficeNumber to schedule your project’s start-date.<br><br>Thank you once again for your time.<br><br>
        We look forward to working with you.
        <p></p>
        <br>
        <br>
        <br>
        <div>
            Regards,
            <div style="display: @Model.HasCustomSignature">
                @Model.Signature
            </div>
            <p>
                @Model.Base.CompanyName<sup>&reg;</sup> <br />
                @Model.OfficeNumber <br />
            </p>
        </div>
    </div>
</div>
</html>'
 where (Id = 50 and NotificationTypeId = 50);
 
 
