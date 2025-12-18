using System.Net.Http.Json;
using ShippingRules.Application.DTOs;

namespace ShippingRules.MAUI.Services;

public class ShippingRulesApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7046/api"; // Update with your API URL

    public ShippingRulesApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public async Task<List<ShippingRuleDto>> GetAllRulesAsync(bool activeOnly = false)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/ShippingRules?activeOnly={activeOnly}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ShippingRuleDto>>() ?? new List<ShippingRuleDto>();
        }
        catch (Exception ex)
        {
            // Log error
            System.Diagnostics.Debug.WriteLine($"Error fetching rules: {ex.Message}");
            return new List<ShippingRuleDto>();
        }
    }

    public async Task<ShippingRuleDto?> CreateRuleAsync(CreateRuleRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/ShippingRules", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShippingRuleDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating rule: {ex.Message}");
            return null;
        }
    }

    public async Task<ShippingRuleDto?> GetApplicableRuleAsync(
        Guid? countryId, Guid? portId, Guid? vesselId, Guid? principalId)
    {
        try
        {
            var query = $"/ShippingRules/applicable?";
            if (countryId.HasValue) query += $"countryId={countryId}&";
            if (portId.HasValue) query += $"portId={portId}&";
            if (vesselId.HasValue) query += $"vesselId={vesselId}&";
            if (principalId.HasValue) query += $"principalId={principalId}&";

            var response = await _httpClient.GetAsync(query.TrimEnd('&'));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShippingRuleDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting applicable rule: {ex.Message}");
            return null;
        }
    }
}

public record CreateRuleRequest(
    string RuleName,
    string Description,
    int PrecedenceLevel,
    Guid? CountryId,
    Guid? PortId,
    Guid? VesselId,
    Guid? PrincipalId,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo,
    decimal BaseRate,
    decimal SurchargePercentage,
    string RuleType,
    string RuleCategory,
    bool RequiresApproval);
