﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Ems.Authorization
{
    public class TenantPermissionConsts
    {
        public const string CustomerAdminPermissions =
                "Pages," +
                "Pages.Administration," +
                "Pages.Administration.OrganizationUnits," +
                "Pages.Administration.OrganizationUnits.ManageMembers," +
                "Pages.Administration.OrganizationUnits.ManageOrganizationTree," +
                "Pages.Administration.OrganizationUnits.ManageRoles," +
                "Pages.Administration.Users," +
                "Pages.Administration.Users.Create," +
                "Pages.Administration.Users.Delete," +
                "Pages.Administration.Users.Edit," +
                "Pages.Administration.Users.Unlock," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.Assets," +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.CustomerInvoices.View," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.Profile," +
                "Pages.Main.Customers.CreateAddress," +
                "Pages.Main.Customers.DeleteAddress," +
                "Pages.Main.Customers.Edit," +
                "Pages.Main.Customers.EditAddress," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Estimates," +
                "Pages.Main.Estimates.View," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseAgreements.View," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportItems," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View";
         
        public const string CustomerUserPermissions =
                "Pages," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.Assets," +
                "Pages.Main.AssetParts," +
                "Pages.Main.AssetParts.ManagePartTree" +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.CustomerInvoices.View," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.Profile," +
                "Pages.Main.Customers.CreateAddress," +
                "Pages.Main.Customers.DeleteAddress," +
                "Pages.Main.Customers.Edit," +
                "Pages.Main.Customers.EditAddress," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Estimates," +
                "Pages.Main.Estimates.View," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseAgreements.View," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportItems," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View";

        public const string VendorAdminPermissions =
                "Pages," +
                "Pages.Administration," +
                "Pages.Administration.OrganizationUnits," +
                "Pages.Administration.OrganizationUnits.ManageMembers," +
                "Pages.Administration.OrganizationUnits.ManageOrganizationTree," +
                "Pages.Administration.OrganizationUnits.ManageRoles," +
                "Pages.Administration.Users," +
                "Pages.Administration.Users.Create," +
                "Pages.Administration.Users.Delete," +
                "Pages.Administration.Users.Edit," +
                "Pages.Administration.Users.Unlock," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.Assets," +
                "Pages.Main.AssetParts," +
                "Pages.Main.AssetParts.ManagePartTree" +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Estimates," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.Quotations," +
                "Pages.Main.Quotations.Create," +
                "Pages.Main.Quotations.Delete," +
                "Pages.Main.Quotations.Edit," +
                "Pages.Main.Quotations.QuotationDetailsCreate," +
                "Pages.Main.Quotations.QuotationDetailsDelete," +
                "Pages.Main.Quotations.QuotationDetailsEdit," +
                "Pages.Main.Quotations.View," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportItems," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.VendorCharges," +
                "Pages.Main.VendorCharges.Create," +
                "Pages.Main.VendorCharges.Delete," +
                "Pages.Main.VendorCharges.Edit," +
                "Pages.Main.VendorCharges.VendorChargeDetailsCreate," +
                "Pages.Main.VendorCharges.VendorChargeDetailsDelete," +
                "Pages.Main.VendorCharges.VendorChargeDetailsEdit," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.Profile," +
                "Pages.Main.Vendors.CreateAddress," +
                "Pages.Main.Vendors.DeleteAddress," +
                "Pages.Main.Vendors.Edit," +
                "Pages.Main.Vendors.EditAddress," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View";

        public const string VendorUserPermissions =
                "Pages," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.Assets," +
                "Pages.Main.AssetParts," +
                "Pages.Main.AssetParts.ManagePartTree" +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Estimates," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.Quotations," +
                "Pages.Main.Quotations.Create," +
                "Pages.Main.Quotations.Delete," +
                "Pages.Main.Quotations.Edit," +
                "Pages.Main.Quotations.QuotationDetailsCreate," +
                "Pages.Main.Quotations.QuotationDetailsDelete," +
                "Pages.Main.Quotations.QuotationDetailsEdit," +
                "Pages.Main.Quotations.View," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportItems," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.VendorCharges," +
                "Pages.Main.VendorCharges.Create," +
                "Pages.Main.VendorCharges.Delete," +
                "Pages.Main.VendorCharges.Edit," +
                "Pages.Main.VendorCharges.VendorChargeDetailsCreate," +
                "Pages.Main.VendorCharges.VendorChargeDetailsDelete," +
                "Pages.Main.VendorCharges.VendorChargeDetailsEdit," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.Profile," +
                "Pages.Main.Vendors.CreateAddress," +
                "Pages.Main.Vendors.DeleteAddress," +
                "Pages.Main.Vendors.Edit," +
                "Pages.Main.Vendors.EditAddress," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View";

        public const string AssetOwnerAdminPermissions =
                "Pages," +
                "Pages.Administration," +
                "Pages.Administration.OrganizationUnits," +
                "Pages.Administration.OrganizationUnits.ManageMembers," +
                "Pages.Administration.OrganizationUnits.ManageOrganizationTree," +
                "Pages.Administration.OrganizationUnits.ManageRoles," +
                "Pages.Administration.Users," +
                "Pages.Administration.Users.Create," +
                "Pages.Administration.Users.Delete," +
                "Pages.Administration.Users.Edit," +
                "Pages.Administration.Users.Unlock," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.Profile," +
                "Pages.Main.AssetOwners.Create," +
                "Pages.Main.AssetOwners.CreateAddress," +
                "Pages.Main.AssetOwners.Delete," +
                "Pages.Main.AssetOwners.DeleteAddress," +
                "Pages.Main.AssetOwners.Edit," +
                "Pages.Main.AssetOwners.EditAddress," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.AssetOwnerships.Create," +
                "Pages.Main.AssetOwnerships.Delete," +
                "Pages.Main.AssetOwnerships.Edit," +
                "Pages.Main.Assets," +
                "Pages.Main.Assets.Create," +
                "Pages.Main.Assets.Delete," +
                "Pages.Main.Assets.Edit," +
                "Pages.Main.AssetParts," +
                "Pages.Main.AssetParts.Create," +
                "Pages.Main.AssetParts.Delete," +
                "Pages.Main.AssetParts.Edit," +
                "Pages.Main.AssetParts.ManagePartTree" +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.BillingEvents," +
                "Pages.Main.BillingEvents.Create," +
                "Pages.Main.BillingEvents.CreateEventDetails," +
                "Pages.Main.BillingEvents.Delete," +
                "Pages.Main.BillingEvents.DeleteEventDetails," +
                "Pages.Main.BillingEvents.Edit," +
                "Pages.Main.BillingEvents.EditEventDetails," +
                "Pages.Main.BillingRules," +
                "Pages.Main.BillingRules.Create," +
                "Pages.Main.BillingRules.Delete," +
                "Pages.Main.BillingRules.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroupMemberships.Create," +
                "Pages.Main.CustomerGroupMemberships.Delete," +
                "Pages.Main.CustomerGroupMemberships.Edit," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerGroups.Create," +
                "Pages.Main.CustomerGroups.Delete," +
                "Pages.Main.CustomerGroups.Edit," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.CustomerInvoices.Create," +
                "Pages.Main.CustomerInvoices.CreateCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.Delete," +
                "Pages.Main.CustomerInvoices.DeleteCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.Edit," +
                "Pages.Main.CustomerInvoices.EditCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.View," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.Create," +
                "Pages.Main.Customers.CreateAddress," +
                "Pages.Main.Customers.Delete," +
                "Pages.Main.Customers.DeleteAddress," +
                "Pages.Main.Customers.Edit," +
                "Pages.Main.Customers.EditAddress," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Dashboard," +
                "Pages.Main.Estimates," +
                "Pages.Main.Estimates.Create," +
                "Pages.Main.Estimates.Delete," +
                "Pages.Main.Estimates.Edit," +
                "Pages.Main.Estimates.View," +
                "Pages.Main.Estimates.EstimateDetailsCreate," +
                "Pages.Main.Estimates.EstimateDetailsDelete," +
                "Pages.Main.Estimates.EstimateDetailsEdit," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseAgreements.Create," +
                "Pages.Main.LeaseAgreements.Delete," +
                "Pages.Main.LeaseAgreements.Edit," +
                "Pages.Main.LeaseAgreements.LeaseItemsCreate," +
                "Pages.Main.LeaseAgreements.LeaseItemsDelete," +
                "Pages.Main.LeaseAgreements.LeaseItemsEdit," +
                "Pages.Main.LeaseAgreements.View," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.Quotations," +
                "Pages.Main.Quotations.Create," +
                "Pages.Main.Quotations.Delete," +
                "Pages.Main.Quotations.Edit," +
                "Pages.Main.Quotations.QuotationDetailsCreate," +
                "Pages.Main.Quotations.QuotationDetailsDelete," +
                "Pages.Main.Quotations.QuotationDetailsEdit," +
                "Pages.Main.Quotations.View," +
                "Pages.Main.Rfqs," +
                "Pages.Main.Rfqs.Create," +
                "Pages.Main.Rfqs.Delete," +
                "Pages.Main.Rfqs.Edit," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportContracts.Create," +
                "Pages.Main.SupportContracts.Delete," +
                "Pages.Main.SupportContracts.Edit," +
                "Pages.Main.SupportItems," +
                "Pages.Main.SupportItems.Create," +
                "Pages.Main.SupportItems.Delete," +
                "Pages.Main.SupportItems.Edit," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetricRecords.Create," +
                "Pages.Main.UsageMetricRecords.Delete," +
                "Pages.Main.UsageMetricRecords.Edit," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.UsageMetrics.Create," +
                "Pages.Main.UsageMetrics.Delete," +
                "Pages.Main.UsageMetrics.Edit," +
                "Pages.Main.VendorCharges," +
                "Pages.Main.VendorCharges.Create," +
                "Pages.Main.VendorCharges.Delete," +
                "Pages.Main.VendorCharges.Edit," +
                "Pages.Main.VendorCharges.VendorChargeDetailsCreate," +
                "Pages.Main.VendorCharges.VendorChargeDetailsDelete," +
                "Pages.Main.VendorCharges.VendorChargeDetailsEdit," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.Create," +
                "Pages.Main.Vendors.CreateAddress," +
                "Pages.Main.Vendors.Delete," +
                "Pages.Main.Vendors.DeleteAddress," +
                "Pages.Main.Vendors.Edit," +
                "Pages.Main.Vendors.EditAddress," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View,";

        public const string AssetOwnerUserPermissions =
                "Pages," +
                "Pages.Configuration," +
                "Pages.Configuration.AssetClasses," +
                "Pages.Configuration.AssetStatuses," +
                "Pages.Configuration.AssetTypes," +
                "Pages.Configuration.BillingEventTypes," +
                "Pages.Configuration.BillingRuleTypes," +
                "Pages.Configuration.ConsumableTypes," +
                "Pages.Configuration.Currencies," +
                "Pages.Configuration.CustomerInvoiceStatuses," +
                "Pages.Configuration.CustomerTypes," +
                "Pages.Configuration.EstimateStatuses," +
                "Pages.Configuration.IncidentPriorities," +
                "Pages.Configuration.IncidentStatuses," +
                "Pages.Configuration.IncidentTypes," +
                "Pages.Main.ItemTypes," +
                "Pages.Configuration.Languages," +
                "Pages.Configuration.QuotationStatuses," +
                "Pages.Configuration.RfqTypes," +
                "Pages.Configuration.SsicCodes," +
                "Pages.Configuration.SupportTypes," +
                "Pages.Configuration.Uoms," +
                "Pages.Configuration.VendorChargeStatuses," +
                "Pages.Configuration.WorkOrderPriorities," +
                "Pages.Configuration.WorkOrderStatuses," +
                "Pages.Configuration.WorkOrderActions," +
                "Pages.Configuration.WorkOrderTypes," +
                "Pages.Main," +
                "Pages.Main.Addresses," +
                "Pages.Main.Addresses.Create," +
                "Pages.Main.Addresses.Delete," +
                "Pages.Main.Addresses.Edit," +
                "Pages.Main.AssetOwners," +
                "Pages.Main.AssetOwners.Profile," +
                "Pages.Main.AssetOwners.Create," +
                "Pages.Main.AssetOwners.CreateAddress," +
                "Pages.Main.AssetOwners.Delete," +
                "Pages.Main.AssetOwners.DeleteAddress," +
                "Pages.Main.AssetOwners.Edit," +
                "Pages.Main.AssetOwners.EditAddress," +
                "Pages.Main.AssetOwners.View," +
                "Pages.Main.AssetOwnerships," +
                "Pages.Main.AssetOwnerships.Create," +
                "Pages.Main.AssetOwnerships.Delete," +
                "Pages.Main.AssetOwnerships.Edit," +
                "Pages.Main.Assets," +
                "Pages.Main.Assets.Create," +
                "Pages.Main.Assets.Delete," +
                "Pages.Main.Assets.Edit," +
                "Pages.Main.Assets.View," +
                "Pages.Main.AssetParts," +
                "Pages.Main.AssetParts.Create," +
                "Pages.Main.AssetParts.Delete," +
                "Pages.Main.AssetParts.Edit," +
                "Pages.Main.AssetParts.ManagePartTree" +
                "Pages.Main.Attachments," +
                "Pages.Main.Attachments.Create," +
                "Pages.Main.Attachments.Delete," +
                "Pages.Main.Attachments.Edit," +
                "Pages.Main.BillingEvents," +
                "Pages.Main.BillingEvents.Create," +
                "Pages.Main.BillingEvents.CreateEventDetails," +
                "Pages.Main.BillingEvents.Delete," +
                "Pages.Main.BillingEvents.DeleteEventDetails," +
                "Pages.Main.BillingEvents.Edit," +
                "Pages.Main.BillingEvents.EditEventDetails," +
                "Pages.Main.BillingRules," +
                "Pages.Main.BillingRules.Create," +
                "Pages.Main.BillingRules.Delete," +
                "Pages.Main.BillingRules.Edit," +
                "Pages.Main.Contacts," +
                "Pages.Main.Contacts.Create," +
                "Pages.Main.Contacts.Delete," +
                "Pages.Main.Contacts.Edit," +
                "Pages.Main.CustomerGroupMemberships," +
                "Pages.Main.CustomerGroupMemberships.Create," +
                "Pages.Main.CustomerGroupMemberships.Delete," +
                "Pages.Main.CustomerGroupMemberships.Edit," +
                "Pages.Main.CustomerGroups," +
                "Pages.Main.CustomerGroups.Create," +
                "Pages.Main.CustomerGroups.Delete," +
                "Pages.Main.CustomerGroups.Edit," +
                "Pages.Main.CustomerInvoices," +
                "Pages.Main.CustomerInvoices.Create," +
                "Pages.Main.CustomerInvoices.CreateCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.Delete," +
                "Pages.Main.CustomerInvoices.DeleteCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.Edit," +
                "Pages.Main.CustomerInvoices.EditCustomerInvoiceDetail," +
                "Pages.Main.CustomerInvoices.View," +
                "Pages.Main.Customers," +
                "Pages.Main.Customers.Create," +
                "Pages.Main.Customers.CreateAddress," +
                "Pages.Main.Customers.Delete," +
                "Pages.Main.Customers.DeleteAddress," +
                "Pages.Main.Customers.Edit," +
                "Pages.Main.Customers.EditAddress," +
                "Pages.Main.Customers.View," +
                "Pages.Main.Dashboard," +
                "Pages.Main.Estimates," +
                "Pages.Main.Estimates.Create," +
                "Pages.Main.Estimates.Delete," +
                "Pages.Main.Estimates.Edit," +
                "Pages.Main.Estimates.View," +
                "Pages.Main.Estimates.EstimateDetailsCreate," +
                "Pages.Main.Estimates.EstimateDetailsDelete," +
                "Pages.Main.Estimates.EstimateDetailsEdit," +
                "Pages.Main.Incidents," +
                "Pages.Main.Incidents.Create," +
                "Pages.Main.Incidents.CreateIncidentUpdates," +
                "Pages.Main.Incidents.Delete," +
                "Pages.Main.Incidents.DeleteIncidentUpdates," +
                "Pages.Main.Incidents.Edit," +
                "Pages.Main.Incidents.EditIncidentUpdates," +
                "Pages.Main.IncidentUpdates," +
                "Pages.Main.IncidentUpdates.Create," +
                "Pages.Main.IncidentUpdates.Delete," +
                "Pages.Main.IncidentUpdates.Edit," +
                "Pages.Main.LeaseAgreements," +
                "Pages.Main.LeaseAgreements.Create," +
                "Pages.Main.LeaseAgreements.Delete," +
                "Pages.Main.LeaseAgreements.Edit," +
                "Pages.Main.LeaseAgreements.LeaseItemsCreate," +
                "Pages.Main.LeaseAgreements.LeaseItemsDelete," +
                "Pages.Main.LeaseAgreements.LeaseItemsEdit," +
                "Pages.Main.LeaseAgreements.View," +
                "Pages.Main.LeaseItems," +
                "Pages.Main.Quotations," +
                "Pages.Main.Quotations.Create," +
                "Pages.Main.Quotations.Delete," +
                "Pages.Main.Quotations.Edit," +
                "Pages.Main.Quotations.QuotationDetailsCreate," +
                "Pages.Main.Quotations.QuotationDetailsDelete," +
                "Pages.Main.Quotations.QuotationDetailsEdit," +
                "Pages.Main.Quotations.View," +
                "Pages.Main.Rfqs," +
                "Pages.Main.Rfqs.Create," +
                "Pages.Main.Rfqs.Delete," +
                "Pages.Main.Rfqs.Edit," +
                "Pages.Main.SupportContracts," +
                "Pages.Main.SupportContracts.Create," +
                "Pages.Main.SupportContracts.Delete," +
                "Pages.Main.SupportContracts.Edit," +
                "Pages.Main.SupportItems," +
                "Pages.Main.SupportItems.Create," +
                "Pages.Main.SupportItems.Delete," +
                "Pages.Main.SupportItems.Edit," +
                "Pages.Main.UsageMetricRecords," +
                "Pages.Main.UsageMetricRecords.Create," +
                "Pages.Main.UsageMetricRecords.Delete," +
                "Pages.Main.UsageMetricRecords.Edit," +
                "Pages.Main.UsageMetrics," +
                "Pages.Main.UsageMetrics.Create," +
                "Pages.Main.UsageMetrics.Delete," +
                "Pages.Main.UsageMetrics.Edit," +
                "Pages.Main.VendorCharges," +
                "Pages.Main.VendorCharges.Create," +
                "Pages.Main.VendorCharges.Delete," +
                "Pages.Main.VendorCharges.Edit," +
                "Pages.Main.VendorCharges.VendorChargeDetailsCreate," +
                "Pages.Main.VendorCharges.VendorChargeDetailsDelete," +
                "Pages.Main.VendorCharges.VendorChargeDetailsEdit," +
                "Pages.Main.Vendors," +
                "Pages.Main.Vendors.Create," +
                "Pages.Main.Vendors.CreateAddress," +
                "Pages.Main.Vendors.Delete," +
                "Pages.Main.Vendors.DeleteAddress," +
                "Pages.Main.Vendors.Edit," +
                "Pages.Main.Vendors.EditAddress," +
                "Pages.Main.Vendors.View," +
                "Pages.Main.WorkOrders," +
                "Pages.Main.WorkOrders.Create," +
                "Pages.Main.WorkOrders.CreateWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Delete," +
                "Pages.Main.WorkOrders.DeleteWorkOrderUpdate," +
                "Pages.Main.WorkOrders.Edit," +
                "Pages.Main.WorkOrders.EditWorkOrderUpdate," +
                "Pages.Main.WorkOrders.View,";


    }
}
