<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CQRS.Sample1.Client.WebMvc.Models.ProductCreation>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AddProduct
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>AddProduct</h2>

    <% using (Html.BeginForm()) {%>
        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Name">Name:</label>
                <%= Html.TextBox("Name", Model.Name) %>
            </p>
            <p>
                <label for="StockCount">StockCount:</label>
                <%= Html.TextBox("StockCount")%>
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>
    <% } %>

</asp:Content>
