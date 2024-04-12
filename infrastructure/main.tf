terraform {
    required_providers {
        azurerm = {
        source  = "hashicorp/azurerm"
        version = "3.80.0"
        }
    }
}

provider "azurerm" {
    features {}
}

resource "azurerm_resource_group" "this" {
    name     = "clippy-was-here-hilarious-cloud"
    location = "eastus"
}

resource "azurerm_service_plan" "this" {
  name                = "clippy-was-here-service-plan"
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  os_type             = "Linux"
  sku_name            = "P1v2"
}

resource "azurerm_linux_web_app" "this" {
    name                = "clippy-was-here-web-app"
    resource_group_name = azurerm_resource_group.this.name
    location            = azurerm_service_plan.this.location
    service_plan_id     = azurerm_service_plan.this.id

    site_config {}
}
