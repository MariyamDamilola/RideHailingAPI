namespace RideHailingAPI.Helpers
{
    public static class MailUtils
    {
        private const string BrandColor = "#2563EB";
        private const string BrandColorDark = "#1E3A8A";
        private const string BrandName = "RideGo";
        private const string SupportEmail = "support@ridego.com";

        // ============================================================
        // MAIN EMAIL WRAPPER
        // ============================================================

        public static string GetEmailWrapper(
            string preheader,
            string title,
            string content)
        {
            var year = DateTime.UtcNow.Year;

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>{title}</title>
</head>

<body style='margin:0; padding:0; background-color:#f3f4f6;
             font-family:Arial, Helvetica, sans-serif;'>

    <!-- Preheader -->
    <div style='display:none; max-height:0; overflow:hidden;
                opacity:0; color:transparent;'>
        {preheader}
    </div>

    <table role='presentation'
           width='100%'
           cellpadding='0'
           cellspacing='0'
           style='background-color:#f3f4f6; padding:30px 0;'>

        <tr>
            <td align='center'>

                <table role='presentation'
                       width='600'
                       cellpadding='0'
                       cellspacing='0'
                       style='width:600px; max-width:600px;
                              background-color:#ffffff;
                              border-radius:12px;
                              overflow:hidden;
                              box-shadow:0 3px 15px rgba(0,0,0,0.08);'>

                    <!-- Top Accent -->
                    <tr>
                        <td style='background-color:{BrandColor};
                                   height:6px;
                                   line-height:6px;
                                   font-size:0;'>
                            &nbsp;
                        </td>
                    </tr>

                    <!-- Header -->
                    <tr>
                        <td style='padding:30px 40px 24px;
                                   text-align:center;
                                   border-bottom:1px solid #eeeeee;'>

                            <div style='font-size:28px;
                                        font-weight:700;
                                        color:{BrandColor};
                                        letter-spacing:0.5px;'>
                                {BrandName}
                            </div>

                            <div style='font-size:11px;
                                        letter-spacing:2px;
                                        text-transform:uppercase;
                                        color:#9ca3af;
                                        margin-top:6px;'>
                                Safe &middot; Reliable &middot; On-Demand Rides
                            </div>

                        </td>
                    </tr>

                    <!-- Title -->
                    <tr>
                        <td style='background-color:{BrandColorDark};
                                   padding:18px 40px;'>

                            <span style='color:#ffffff;
                                         font-size:16px;
                                         font-weight:600;'>
                                {title}
                            </span>

                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style='padding:40px;
                                   color:#333333;
                                   font-size:15px;
                                   line-height:1.65;'>

                            {content}

                        </td>
                    </tr>

                    <!-- Divider -->
                    <tr>
                        <td style='padding:0 40px;'>
                            <div style='border-top:1px solid #eeeeee;'></div>
                        </td>
                    </tr>

                    <!-- Support -->
                    <tr>
                        <td style='padding:24px 40px;
                                   text-align:center;'>

                            <span style='font-size:13px;
                                         color:#777777;'>
                                Need help with your account or ride?
                                Contact us at
                                <a href='mailto:{SupportEmail}'
                                   style='color:{BrandColor};
                                          text-decoration:none;
                                          font-weight:600;'>
                                    {SupportEmail}
                                </a>
                            </span>

                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color:#fafafa;
                                   padding:24px 40px;
                                   text-align:center;
                                   border-top:1px solid #eeeeee;'>

                            <div style='font-size:12px;
                                        color:#9ca3af;
                                        line-height:1.6;'>

                                &copy; {year} {BrandName}. All rights reserved.<br/>

                                This is an automated notification.
                                Please do not reply directly to this email.

                            </div>

                        </td>
                    </tr>

                    <!-- Bottom Accent -->
                    <tr>
                        <td style='background-color:{BrandColor};
                                   height:6px;
                                   line-height:6px;
                                   font-size:0;'>
                            &nbsp;
                        </td>
                    </tr>

                </table>

            </td>
        </tr>

    </table>

</body>
</html>";
        }
        

        private static string OtpCardBlock(string otpCode, string label)
        {
            return $@"
<table role='presentation'
       width='100%'
       cellpadding='0'
       cellspacing='0'
       style='margin:20px 0 28px;'>

    <tr>
        <td align='center'
            style='background-color:{BrandColorDark};
                   border-radius:10px;
                   padding:28px;'>

            <div style='font-size:11px;
                        letter-spacing:2px;
                        text-transform:uppercase;
                        color:#bfdbfe;
                        margin-bottom:8px;'>
                {label}
            </div>

            <div style='font-family:Courier New, monospace;
                        font-size:36px;
                        font-weight:700;
                        color:#ffffff;
                        letter-spacing:8px;
                        margin-bottom:10px;'>
                {otpCode}
            </div>

            <div style='font-size:13px;
                        color:#dbeafe;'>
                Valid for <strong>5 minutes</strong>.
                Do not share this code with anyone.
            </div>

        </td>
    </tr>

</table>";
        }



        private static string DetailRow(
            string label,
            string value,
            bool shaded)
        {
            var bg = shaded ? "#f9fafb" : "#ffffff";

            return $@"
<tr>

    <td style='padding:12px 16px;
               background-color:{bg};
               border-bottom:1px solid #eeeeee;
               font-size:13px;
               color:#6b7280;
               font-weight:600;
               width:42%;'>
        {label}
    </td>

    <td style='padding:12px 16px;
               background-color:{bg};
               border-bottom:1px solid #eeeeee;
               font-size:14px;
               color:#1f2937;'>
        {value}
    </td>

</tr>";
        }


        private static string DetailsTable(string rows)
        {
            return $@"
<table role='presentation'
       width='100%'
       cellpadding='0'
       cellspacing='0'
       style='margin:24px 0;
              border:1px solid #eeeeee;
              border-radius:8px;
              overflow:hidden;'>

    {rows}

</table>";
        }

        

        public static string GetRegistrationOtpEmailHtml(
            string fullName,
            string userId,
            string otpCode)
        {
            var title = "Verify Your RideGo Account";

            var preheader =
                $"Your RideGo verification code is {otpCode}.";

            var detailsRows = string.Concat(

                DetailRow(
                    "User ID",
                    userId,
                    false),

                DetailRow(
                    "Account Status",
                    "<strong style='color:#d97706;'>PENDING VERIFICATION</strong>",
                    true),

                DetailRow(
                    "Requested At",
                    $"{DateTime.UtcNow:f} UTC",
                    false)
            );

            var content = $@"
<div style='text-align:center; margin-bottom:25px;'>

    <div style='font-size:23px;
                font-weight:700;
                color:#111827;
                margin-bottom:8px;'>
        Welcome to {BrandName}, {fullName}!
    </div>

    <div style='font-size:14px;
                color:#6b7280;'>
        Thanks for creating your account.
        Use the verification code below to confirm your email address.
    </div>

</div>

{OtpCardBlock(otpCode, "Email Verification Code")}

<div style='font-size:14px;
            color:#374151;
            margin-bottom:5px;
            font-weight:700;'>
    Account Information
</div>

{DetailsTable(detailsRows)}

<p style='font-size:13px; color:#6b7280;'>
    If you did not create a RideGo account, you can safely ignore this email.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }

        

        public static string GetResendOtpEmailHtml(
            string fullName,
            string otpCode)
        {
            var title = "Your New Verification Code";

            var preheader =
                $"Your new RideGo verification code is {otpCode}.";

            var content = $@"
<div style='font-size:21px;
            font-weight:700;
            color:#111827;
            margin-bottom:10px;'>
    New Verification Code
</div>

<p style='color:#555555; margin-top:0;'>
    Hello {fullName}, you requested a new verification code.
    Please use the code below to verify your account.
</p>

{OtpCardBlock(otpCode, "New Verification Code")}

<p style='font-size:13px; color:#6b7280;'>
    Your previous verification code is no longer valid.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 3. EMAIL VERIFIED
        // ============================================================

        public static string GetAccountVerifiedEmailHtml(
            string fullName,
            string userId)
        {
            var title = "Account Successfully Verified";

            var preheader =
                $"Your RideGo account has been verified.";

            var detailsRows = string.Concat(

                DetailRow(
                    "User ID",
                    userId,
                    false),

                DetailRow(
                    "Account Status",
                    "<strong style='color:#16a34a;'>VERIFIED</strong>",
                    true),

                DetailRow(
                    "Verified At",
                    $"{DateTime.UtcNow:f} UTC",
                    false)
            );

            var content = $@"
<div style='text-align:center;'>

    <div style='font-size:23px;
                font-weight:700;
                color:#111827;
                margin-bottom:10px;'>
        Email Verified Successfully
    </div>

    <p style='color:#555555;'>
        Hello {fullName}, your email address has been successfully
        verified. Your RideGo account is now ready for use.
    </p>

</div>

{DetailsTable(detailsRows)}

<p style='font-size:14px; color:#555555;'>
    You can now log in and start using RideGo.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 4. PASSWORD RESET OTP
        // ============================================================

        public static string GetPasswordResetOtpEmailHtml(
            string fullName,
            string otpCode)
        {
            var title = "Password Reset Request";

            var preheader =
                $"Your RideGo password reset code is {otpCode}.";

            var content = $@"
<div style='font-size:21px;
            font-weight:700;
            color:#111827;
            margin-bottom:10px;'>
    Reset Your Password
</div>

<p style='color:#555555;'>
    Hello {fullName}, we received a request to reset the password
    for your RideGo account.
</p>

{OtpCardBlock(otpCode, "Password Reset Code")}

<div style='background-color:#fff7ed;
            border-left:4px solid #f97316;
            padding:14px 16px;
            margin-top:20px;
            color:#7c2d12;
            font-size:13px;'>
    If you did not request a password reset, please ignore this email
    and make sure your account credentials remain secure.
</div>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }
        
        public static string GetAccountDeactivatedEmailHtml(
            string firstName,
            string reason)
        {
            var title = "Account Deactivated";

            var preheader =
                "Your RideGo account has been successfully deactivated.";

            var content = $@"
<div style='text-align:center; margin-bottom:20px;'>

    <div style='font-size:22px;
                font-weight:700;
                color:#111827;'>
        Account Deactivated
    </div>

    <p style='color:#555555;'>
        Hello {firstName}, your RideGo account has been
        successfully deactivated.
    </p>

</div>

<p style='font-size:14px; color:#555555;'>
    <strong>Reason:</strong> {reason}
</p>

<p style='font-size:14px; color:#555555;'>
    If you did not request this change, please contact
    RideGo support.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 5. PASSWORD CHANGED
        // ============================================================

        public static string GetPasswordChangedEmailHtml(
            string fullName,
            string userId)
        {
            var title = "Security Alert · Password Changed";

            var preheader =
                "Your RideGo password was successfully changed.";

            var detailsRows = string.Concat(

                DetailRow(
                    "User ID",
                    userId,
                    false),

                DetailRow(
                    "Action",
                    "Password Changed",
                    true),

                DetailRow(
                    "Timestamp",
                    $"{DateTime.UtcNow:f} UTC",
                    false)
            );

            var content = $@"
<div style='font-size:21px;
            font-weight:700;
            color:#111827;
            margin-bottom:10px;'>
    Password Updated
</div>

<p style='color:#555555;'>
    Hello {fullName}, your RideGo password was successfully changed.
</p>

{DetailsTable(detailsRows)}

<div style='background-color:#fef2f2;
            border-left:4px solid #dc2626;
            padding:14px 16px;
            color:#7f1d1d;
            font-size:13px;'>
    If you did not make this change, please contact RideGo support
    immediately.
</div>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 6. DRIVER APPROVED
        // ============================================================

        public static string GetDriverApprovedEmailHtml(
            string driverName,
            string driverId)
        {
            var title = "Driver Application Approved";

            var preheader =
                "Your RideGo driver application has been approved.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Driver ID",
                    driverId,
                    false),

                DetailRow(
                    "Application Status",
                    "<strong style='color:#16a34a;'>APPROVED</strong>",
                    true),

                DetailRow(
                    "Approved At",
                    $"{DateTime.UtcNow:f} UTC",
                    false)
            );

            var content = $@"
<div style='text-align:center; margin-bottom:20px;'>

    <div style='font-size:23px;
                font-weight:700;
                color:#111827;'>
        Congratulations, {driverName}!
    </div>

    <p style='color:#555555;'>
        Your RideGo driver application has been approved by an administrator.
        You can now go online and accept available ride requests.
    </p>

</div>

{DetailsTable(detailsRows)}

<p style='font-size:14px; color:#555555;'>
    Keep your driver and vehicle information up to date and always
    follow RideGo's safety requirements.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 7. DRIVER REJECTED
        // ============================================================

        public static string GetDriverRejectedEmailHtml(
            string driverName,
            string driverId,
            string reason)
        {
            var title = "Driver Application Update";

            var preheader =
                "There has been an update to your RideGo driver application.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Driver ID",
                    driverId,
                    false),

                DetailRow(
                    "Application Status",
                    "<strong style='color:#dc2626;'>REJECTED</strong>",
                    true),

                DetailRow(
                    "Reason",
                    reason,
                    false),

                DetailRow(
                    "Reviewed At",
                    $"{DateTime.UtcNow:f} UTC",
                    true)
            );

            var content = $@"
<div style='font-size:21px;
            font-weight:700;
            color:#111827;
            margin-bottom:10px;'>
    Driver Application Update
</div>

<p style='color:#555555;'>
    Hello {driverName}, your RideGo driver application has been reviewed.
</p>

{DetailsTable(detailsRows)}

<p style='font-size:14px; color:#555555;'>
    Please review the reason above and contact support if you need
    additional information.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 8. DRIVER ACCEPTED RIDE
        // ============================================================

        public static string GetRideAcceptedEmailHtml(
            string passengerName,
            string driverName,
            string rideReference)
        {
            var title = "Your Ride Has Been Accepted";

            var preheader =
                $"{driverName} has accepted your RideGo ride.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Ride Reference",
                    rideReference,
                    false),

                DetailRow(
                    "Driver",
                    driverName,
                    true),

                DetailRow(
                    "Ride Status",
                    "<strong style='color:#2563eb;'>ACCEPTED</strong>",
                    false),

                DetailRow(
                    "Accepted At",
                    $"{DateTime.UtcNow:f} UTC",
                    true)
            );

            var content = $@"
<div style='text-align:center; margin-bottom:20px;'>

    <div style='font-size:22px;
                font-weight:700;
                color:#111827;'>
        Your Driver Is On The Way
    </div>

    <p style='color:#555555;'>
        Hello {passengerName}, your ride request has been accepted.
        Your driver will be heading to your pickup location.
    </p>

</div>

{DetailsTable(detailsRows)}

<p style='font-size:14px; color:#555555;'>
    Please be ready at your pickup location.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 9. DRIVER ARRIVED
        // ============================================================

        public static string GetDriverArrivedEmailHtml(
            string passengerName,
            string driverName,
            string rideReference)
        {
            var title = "Your Driver Has Arrived";

            var preheader =
                $"{driverName} has arrived at your pickup location.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Ride Reference",
                    rideReference,
                    false),

                DetailRow(
                    "Driver",
                    driverName,
                    true),

                DetailRow(
                    "Ride Status",
                    "<strong style='color:#16a34a;'>DRIVER ARRIVED</strong>",
                    false),

                DetailRow(
                    "Arrival Time",
                    $"{DateTime.UtcNow:f} UTC",
                    true)
            );

            var content = $@"
<div style='text-align:center; margin-bottom:20px;'>

    <div style='font-size:22px;
                font-weight:700;
                color:#111827;'>
        Your Driver Has Arrived
    </div>

    <p style='color:#555555;'>
        Hello {passengerName}, your driver has arrived at the pickup
        location and is waiting for you.
    </p>

</div>

{DetailsTable(detailsRows)}";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }


        // ============================================================
        // 10. RIDE CANCELLED
        // ============================================================

        public static string GetRideCancelledEmailHtml(
            string passengerName,
            string rideReference,
            string cancelledBy,
            string reason)
        {
            var title = "Ride Cancelled";

            var preheader =
                $"Ride {rideReference} has been cancelled.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Ride Reference",
                    rideReference,
                    false),

                DetailRow(
                    "Cancelled By",
                    cancelledBy,
                    true),

                DetailRow(
                    "Status",
                    "<strong style='color:#dc2626;'>CANCELLED</strong>",
                    false),

                DetailRow(
                    "Reason",
                    reason,
                    true),

                DetailRow(
                    "Cancelled At",
                    $"{DateTime.UtcNow:f} UTC",
                    false)
            );

            var content = $@"
<div style='font-size:21px;
            font-weight:700;
            color:#111827;
            margin-bottom:10px;'>
    Ride Cancelled
</div>

<p style='color:#555555;'>
    Hello {passengerName}, your RideGo ride has been cancelled.
</p>

{DetailsTable(detailsRows)}

<p style='font-size:13px; color:#6b7280;'>
    If you believe this cancellation was made incorrectly,
    please contact our support team.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }

        

        public static string GetRideCompletedEmailHtml(
            string passengerName,
            string rideReference,
            string driverName)
        {
            var title = "Ride Completed";

            var preheader =
                $"Your RideGo ride {rideReference} has been completed.";

            var detailsRows = string.Concat(

                DetailRow(
                    "Ride Reference",
                    rideReference,
                    false),

                DetailRow(
                    "Driver",
                    driverName,
                    true),

                DetailRow(
                    "Ride Status",
                    "<strong style='color:#16a34a;'>COMPLETED</strong>",
                    false),

                DetailRow(
                    "Completed At",
                    $"{DateTime.UtcNow:f} UTC",
                    true)
            );

            var content = $@"
<div style='text-align:center; margin-bottom:20px;'>

    <div style='font-size:22px;
                font-weight:700;
                color:#111827;'>
        Thanks for Riding With Us!
    </div>

    <p style='color:#555555;'>
        Hello {passengerName}, your ride has been completed successfully.
        We hope you had a safe and comfortable trip.
    </p>

</div>

{DetailsTable(detailsRows)}

<p style='font-size:14px; color:#555555;'>
    Thank you for choosing {BrandName}.
</p>";

            return GetEmailWrapper(
                preheader,
                title,
                content);
        }
    }
    
  



}