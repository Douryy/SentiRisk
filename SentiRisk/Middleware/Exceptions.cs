namespace SentiRisk.Exceptions
{
    /// <summary>Exception de base — toutes les erreurs métier en héritent.</summary>
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>Ressource introuvable (404).</summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string resource, object id)
            : base($"{resource} introuvable (id: {id})", 404) { }

        public NotFoundException(string message)
            : base(message, 404) { }
    }

    /// <summary>Données invalides ou règle métier violée (422).</summary>
    public class ValidationException : AppException
    {
        public ValidationException(string message)
            : base(message, 422) { }
    }

    /// <summary>Utilisateur non authentifié (401).</summary>
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Authentification requise.")
            : base(message, 401) { }
    }

    /// <summary>Accès refusé malgré l'authentification (403).</summary>
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Accès refusé.")
            : base(message, 403) { }
    }

    /// <summary>Conflit de données (ex: ticker déjà existant) (409).</summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, 409) { }
    }

    // ── Exceptions spécifiques à SentiRisk ──────────────────

    /// <summary>Asset financier introuvable.</summary>
    public class AssetNotFoundException : NotFoundException
    {
        public AssetNotFoundException(string ticker)
            : base($"L'asset avec le ticker '{ticker}' est introuvable.") { }
    }

    /// <summary>Portfolio introuvable ou non accessible.</summary>
    public class PortfolioNotFoundException : NotFoundException
    {
        public PortfolioNotFoundException(int portfolioId)
            : base("Portfolio", portfolioId) { }
    }

    /// <summary>Score de sentiment invalide (doit être entre -1 et 1).</summary>
    public class InvalidSentimentScoreException : ValidationException
    {
        public InvalidSentimentScoreException(decimal score)
            : base($"Le score de sentiment '{score}' est invalide. Il doit être compris entre -1 et 1.") { }
    }

    /// <summary>La somme des poids d'un portfolio doit être égale à 1.0.</summary>
    public class InvalidPortfolioWeightException : ValidationException
    {
        public InvalidPortfolioWeightException(decimal totalWeight)
            : base($"La somme des poids du portfolio est {totalWeight:F2}. Elle doit être égale à 1.0.") { }
    }
}