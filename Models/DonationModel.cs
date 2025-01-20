using System;

namespace BBMS_WebAPI.Models
{
    public class DonationModel
    {
        public int DonationID { get; set; } // Optional for insert

        public string DonorName { get; set; } // for select op

        public string BloodGroupName { get; set; } // Required, Foreign Key to BloodGroupMaster table

        public int Quantity { get; set; } // Required, must be greater than 0

        public int Weight { get; set; } // Required, must be greater than 0

        public DateTime? LastDonationDate { get; set; } = DateTime.Now; // Optional, nullable for new donors

        public string? Disease { get; set; } // Optional

        public bool IsEligible { get; set; } // Required, determines eligibility

        public string Status { get; set; } = "Pending"; // Default is 'Pending'

        public DateTime DateOfDonation { get; set; } = DateTime.Now; // Auto-set to current date

        public string? CertificatePath { get; set; } // Optional

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Auto-set to current date

        public DateTime? UpdatedAt { get; set; } // Optional
    }

    public class DonationUpdateStatusModel
    {
        public int DonationID { get; set; } // Required, primary key for identification

        public string NewStatus { get; set; } // Required, values: 'Approved', 'Rejected'
    }

    public class DonationDropDownModel
    {
        public int DonationID { get; set; } // Primary key
        public string DonorName { get; set; } // Donor's name for display
        public string BloodGroupName { get; set; } // Blood group name for display
    }
}
