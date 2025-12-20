@page "/"
@inject HttpClient Http

<PageTitle>Home</PageTitle>

<div class="container mt-4">
    <h2>Welcome to the Shipping Rules App</h2>
    <p class="lead">Manage your shipping rules with ease.</p>

    <div class="row">
        <div class="col-md-4">
            <div class="card h-100">
                <div class="card-body text-center">
                    <h5 class="card-title">📦 Shipping Rules</h5>
                    <p class="card-text">Define and manage your shipping rules</p>
                    <a href="/shipping-rules" class="btn btn-primary">Go to Shipping Rules</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card h-100">
                <div class="card-body text-center">
                    <h5 class="card-title">🌍 Countries</h5>
                    <p class="card-text">View all active countries from master data</p>
                    <a href="/countries" class="btn btn-outline-primary">Browse Countries</a>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="card h-100">
                <div class="card-body text-center">
                    <h5 class="card-title">📊 Reports</h5>
                    <p class="card-text">Generate and view shipping reports</p>
                    <a href="/reports" class="btn btn-secondary">View Reports</a>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    protected override async Task OnInitializedAsync()
    {
        var usa = new Country { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Code = "US", Name = "United States", Region = "North America", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var china = new Country { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Code = "CN", Name = "China", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var india = new Country { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Code = "IN", Name = "India", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var singapore = new Country { Id = Guid.Parse("44444444-1111-2222-3333-444444444444"), Code = "SG", Name = "Singapore", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };
        var malaysia = new Country { Id = Guid.Parse("55555555-1111-2222-3333-555555555555"), Code = "MY", Name = "Malaysia", Region = "Asia", IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" };

        modelBuilder.Entity<Country>().HasData(usa, china, india, singapore, malaysia);
    }
}