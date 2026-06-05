namespace BlankCanvasApp.Domain.Emuns
{
    public class Constants
    {
        public enum CustomerStatus
        {
            Newlead = 1,
            ProposalSent,
            FollowUp,
            Negotiation,
            Active,
            NoResponse,
            Lost
        }
        public static class CustomerStatusMeta
        {
            public record StatusInfo(string Label, string CssClass, string Color);

            public static readonly Dictionary<CustomerStatus, StatusInfo> Data = new()
            {
                [CustomerStatus.Newlead] = new("Nuevo lead", "status-nuevo-lead", "#9E9E9E"),
                [CustomerStatus.ProposalSent] = new("Propuesta enviada", "status-propuesta-enviada", "#5C9BD6"),
                [CustomerStatus.FollowUp] = new("Follow-up", "status-followup", "#F4C430"),
                [CustomerStatus.Negotiation] = new("Negociación", "status-negociacion", "#E07B39"),
                [CustomerStatus.Active] = new("Cliente activo", "status-cliente-activo", "#4CAF50"),
                [CustomerStatus.NoResponse] = new("Sin respuesta", "status-sin-respuesta", "#424242"),
                [CustomerStatus.Lost] = new("Perdido", "status-perdido", "#E53935"),
            };

            /// <summary>Obtiene el label legible del estado.</summary>
            public static string GetLabel(CustomerStatus status)
                => Data.TryGetValue(status, out var info) ? info.Label : status.ToString();

            /// <summary>Obtiene la clase CSS del estado.</summary>
            public static string GetCssClass(CustomerStatus status)
                => Data.TryGetValue(status, out var info) ? info.CssClass : "";
        }
    }
}

