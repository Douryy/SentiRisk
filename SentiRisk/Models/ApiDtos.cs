using System;
using System.Collections.Generic;

namespace SentiRisk.Models
{
    // ===== API DTOs (lecture) =====
    public class ApiAssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Ticker { get; set; } = "";
        public string Sector { get; set; } = "";
        public decimal CurrentPrice { get; set; }
    }

    public class ApiPortfolioAssetDto
    {
        public decimal Weight { get; set; }
        public ApiAssetDto? Asset { get; set; }
    }

    public class ApiPortfolioDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int UserId { get; set; }
        public List<ApiPortfolioAssetDto>? PortfolioAssets { get; set; }
    }

    public class ApiUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public ApiRoleDto? Role { get; set; }
    }

    public class ApiRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class ApiScenarioDto
    {
        public int Id { get; set; }
        public string ScenarioName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal ImpactFactor { get; set; }
        public string TargetSector { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class ApiNewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime PublishedDate { get; set; }
        public int AssetId { get; set; }
        public ApiAssetDto? Asset { get; set; }
        public List<ApiSentimentScoreDto>? SentimentScores { get; set; }
    }

    public class ApiSentimentScoreDto
    {
        public int Id { get; set; }
        public decimal Score { get; set; }
        public decimal Confidence { get; set; }
        public int NewsId { get; set; }
    }

    // ===== API DTOs (écriture - create / upsert) =====
    public class ApiAssetCreateDto
    {
        public string Name { get; set; } = "";
        public string Ticker { get; set; } = "";
        public string Sector { get; set; } = "";
        public decimal CurrentPrice { get; set; }
    }

    public class ApiPortfolioAssetCreateDto
    {
        public int AssetId { get; set; }
        public decimal Weight { get; set; }
    }

    public class ApiPortfolioCreateDto
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int UserId { get; set; }
        public List<ApiPortfolioAssetCreateDto>? PortfolioAssets { get; set; }
    }

    public class ApiPortfolioAssetUpsertDto
    {
        public int PortfolioId { get; set; }
        public int AssetId { get; set; }
        public decimal Weight { get; set; }
    }

    public class ApiUserCreateDto
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public int RoleId { get; set; }
    }

    public class ApiRoleCreateDto
    {
        public string Name { get; set; } = "";
    }

    public class ApiScenarioCreateDto
    {
        public string ScenarioName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal ImpactFactor { get; set; }
        public string TargetSector { get; set; } = "";
    }

    public class ApiNewsCreateDto
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime? PublishedDate { get; set; }
        public int AssetId { get; set; }
    }

    public class ApiSentimentScoreCreateDto
    {
        public decimal Score { get; set; }
        public decimal Confidence { get; set; }
        public int NewsId { get; set; }
    }
}