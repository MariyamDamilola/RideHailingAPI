namespace RideHailingAPI.Helpers;

 public static class SmsUtils
    {
        // ============================================================
        // 1. AUTHENTICATION & SECURITY
        // ============================================================

        public static string GetRegistrationSms(
            string firstName,
            int userId,
            string otpCode)
        {
            return $"Hello {firstName}, welcome to RideGo! " +
                   $"Your User ID is {userId}. " +
                   $"Your email verification OTP is: {otpCode}. " +
                   $"Expires in 5 mins.";
        }


        public static string GetResendOtpSms(
            string firstName,
            string otpCode)
        {
            return $"Hello {firstName}, your new RideGo verification OTP is: " +
                   $"{otpCode}. Valid for 5 mins. Do not share this code.";
        }


        public static string GetPhoneVerificationSms(
            string firstName,
            string otpCode)
        {
            return $"Hello {firstName}, your RideGo phone verification OTP is: " +
                   $"{otpCode}. Valid for 5 mins. Do not share this code.";
        }


        public static string GetVerificationSuccessSms(
            string firstName,
            int userId)
        {
            return $"Hello {firstName}, your RideGo account ({userId}) " +
                   $"has been successfully verified and activated. " +
                   $"You can now log in.";
        }


        public static string GetPasswordChangedSms(
            string firstName)
        {
            return $"Security Alert: Hello {firstName}, your RideGo password " +
                   $"was changed successfully. If you did not initiate this, " +
                   $"contact support immediately.";
        }


        // ============================================================
        // 2. DRIVER MANAGEMENT
        // ============================================================

        public static string GetDriverApprovedSms(
            string firstName,
            int driverId)
        {
            return $"Congratulations {firstName}! Your RideGo driver " +
                   $"application ({driverId}) has been approved. " +
                   $"You can now go online and accept rides.";
        }


        public static string GetDriverRejectedSms(
            string firstName,
            int driverId,
            string reason)
        {
            return $"Hello {firstName}, your RideGo driver application " +
                   $"({driverId}) was not approved. Reason: {reason}. " +
                   $"Please contact support for more information.";
        }


        public static string GetDriverAvailabilitySms(
            string firstName,
            string status)
        {
            return $"RideGo Driver Update: Hello {firstName}, your driver " +
                   $"availability is now set to {status}.";
        }


        // ============================================================
        // 3. RIDE MANAGEMENT
        // ============================================================

        public static string GetRideRequestedSms(
            string passengerName,
            string rideReference)
        {
            return $"Ride Requested: Hello {passengerName}, your RideGo " +
                   $"ride request ({rideReference}) has been received. " +
                   $"We are looking for an available driver.";
        }


        public static string GetRideAcceptedSms(
            string passengerName,
            string driverName,
            string rideReference)
        {
            return $"Ride Accepted: Hello {passengerName}, {driverName} " +
                   $"has accepted your RideGo ride ({rideReference}). " +
                   $"Your driver is on the way.";
        }


        public static string GetDriverArrivingSms(
            string passengerName,
            string driverName,
            string rideReference)
        {
            return $"Driver Arriving: Hello {passengerName}, your driver " +
                   $"{driverName} is heading to your pickup location. " +
                   $"Ride: {rideReference}.";
        }


        public static string GetDriverArrivedSms(
            string passengerName,
            string driverName,
            string rideReference)
        {
            return $"Driver Arrived: Hello {passengerName}, {driverName} " +
                   $"has arrived at your pickup location. " +
                   $"Ride: {rideReference}.";
        }


        public static string GetRideStartedSms(
            string passengerName,
            string rideReference)
        {
            return $"Ride Started: Hello {passengerName}, your RideGo ride " +
                   $"({rideReference}) is now in progress. Have a safe trip!";
        }


        public static string GetRideCompletedSms(
            string passengerName,
            string rideReference)
        {
            return $"Ride Completed: Hello {passengerName}, your RideGo " +
                   $"ride ({rideReference}) has been completed successfully. " +
                   $"Thank you for riding with us!";
        }


        public static string GetRideCancelledSms(
            string passengerName,
            string rideReference,
            string cancelledBy)
        {
            return $"Ride Cancelled: Hello {passengerName}, your RideGo " +
                   $"ride ({rideReference}) has been cancelled by " +
                   $"{cancelledBy}. Please request another ride if needed.";
        }


        // ============================================================
        // 4. DRIVER RIDE NOTIFICATIONS
        // ============================================================

        public static string GetNewRideRequestDriverSms(
            string driverName,
            string rideReference)
        {
            return $"New Ride Request: Hello {driverName}, a new RideGo " +
                   $"ride request ({rideReference}) is available. " +
                   $"Open the app to view the request.";
        }


        public static string GetRideRejectedDriverSms(
            string driverName,
            string rideReference)
        {
            return $"Ride Update: Hello {driverName}, ride request " +
                   $"({rideReference}) has been rejected or is no longer available.";
        }


        // ============================================================
        // 5. PROFILE & ACCOUNT MANAGEMENT
        // ============================================================

        public static string GetProfileUpdatedSms(
            string firstName,
            int userId)
        {
            return $"Profile Alert: Hello {firstName}, your RideGo user " +
                   $"profile ({userId}) was successfully updated.";
        }


        public static string GetAccountDeactivatedSms(
            string firstName,
            int userId)
        {
            return $"Notice: Hello {firstName}, your RideGo account " +
                   $"({userId}) has been deactivated by system administration.";
        }


        public static string GetAccountActivatedSms(
            string firstName,
            int userId)
        {
            return $"Notice: Hello {firstName}, your RideGo account " +
                   $"({userId}) has been activated successfully.";
        }


        // ============================================================
        // 6. SECURITY
        // ============================================================

        public static string GetPasswordResetSms(
            string firstName,
            string otpCode)
        {
            return $"Hello {firstName}, your RideGo password reset OTP is " +
                   $"{otpCode}. It expires in 5 mins. Do not share this code.";
        }


        public static string GetSecurityAlertSms(
            string firstName,
            string message)
        {
            return $"RideGo Security Alert: Hello {firstName}, {message}";
        }
    }  
