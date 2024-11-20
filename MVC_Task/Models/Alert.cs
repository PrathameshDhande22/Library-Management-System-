using System;

namespace MVC_Task.Models
{
    /// <summary>
    /// Used to Show the Alert in The View used by Bootstrap Alert.
    /// </summary>
    public class Alert
    {

        public Alert(string message, Status status)
        {
            this.Message = message;
            this.Status = status;
        }

        public Alert() { }

        public string Message { get; set; }
        public Status Status { get; set; }

        /// <summary>
        /// Converts the <see langword="enum"/> Status to String Status.
        /// </summary>
        public string BsStatus
        {
            get
            {
                return Enum.GetName(typeof(Status), Status).ToLower();
            }
        }
    }

    /// <summary>
    /// Indication of the Status Which can be either Danger, Success or Warning.
    /// </summary>
    public enum Status
    {
        Success,
        Warning,
        Danger,
    }
}