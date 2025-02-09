using System;

namespace BBMS_WebAPI.Models
{
    public class BloodRequestModel
    {
        public int RequestID { get; set; } // Optional for insert

        public string RecipientName { get; set; } // Required for display

        public string BloodGroupName { get; set; } // Required, Foreign Key to BloodGroupMaster table

        public int Quantity { get; set; } // Required, must be greater than 0

        public string Reason { get; set; } // Optional, reason for blood request

        public string Status { get; set; } = "Pending"; // Default to 'Pending'

        public DateTime RequestDate { get; set; } = DateTime.Now; // Required, date when blood request is made

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Auto-set on creation

        public DateTime? UpdatedAt { get; set; } // Optional, updated timestamp
    }

    public class BloodRequestUpdateStatusModel
    {
        public int RequestID { get; set; } // Required, primary key for identification

        public string NewStatus { get; set; } // Required, values: 'Approved', 'Rejected'
    }

    public class BloodRequestDropDownModel
    {
        public int RequestID { get; set; } // Primary key
        public string RecipientName { get; set; } // Recipient's name for display
        public string BloodGroupName { get; set; } // Blood group name for display
    }
}
