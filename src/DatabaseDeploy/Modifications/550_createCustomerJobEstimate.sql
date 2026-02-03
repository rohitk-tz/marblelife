ALTER TABLE `beforeafterimages` CHANGE COLUMN `IsBestImage` `IsBestImage` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `beforeafterimages` CHANGE COLUMN `IsAddToLocalGallery` `IsAddToLocalGallery` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `beforeafterimages` 
ADD COLUMN `IsImagePairReviewed` Bit(1) DEFAULT b'0';

insert into notificationtype(Title, Description, IsQueuingEnabled, IsServiceEnabled, IsDeleted)
values('Weekly Notification Of Photo Management', 'Weekly Notification Of Photo Management', 1, 1, 0);

Insert into emailtemplate (Id, NotificationTypeId, Title, Description, Subject, Body, IsRequired, IsDeleted, languageId, isActive)
Values (583, 63, 'Weekly Notification Of Photo Management', 'Weekly Notification Of Photo Management', 
'Weekly Notification Of Photo Management',
'<!DOCTYPE html
    PUBLIC " -//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>MARBLELIFE</title>
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
    div,
    p,
    ul,
    h1 {
        margin: 0;
    }

    .bgBody {
        background: #ffffff;
    }

    .block {
        display: block;
    }

    .none {
        display: none;
    }
</style>

<div class="contentEditableContainer contentTextEditable">
    <div class="contentEditable">
        <span>Dear Admin,</span>
        <br>
        <br>
        <p>
            Please find the PHOTO MGMT (LOCAL REVIEW – PAYROLL PREP) page link with the data of photo report who have uploaded their
            Before/After images for last week. This link have the report from @Model.WeeklyPhotoManagementModel.StartDate to @Model.WeeklyPhotoManagementModel.EndDate for all the Franchisee.
            <br> <br>
        </p>
        <div class="block">
            <a href="@Model.WeeklyPhotoManagementModel.LocalMarketingURL" target="_blank">Click here</a>
        </div>
        <p></p>
        <br>
        <br>
        <br>
        <div>
            Thank You!
            <div style="display: none">

            </div>
            <p>
                MARBLELIFE<sup>&reg;</sup> <br />
            </p>
        </div>
    </div>
</div>
</html>', b'1', b'0', 249, b'1');