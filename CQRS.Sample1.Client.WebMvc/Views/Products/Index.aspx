<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CQRS.Sample1.Process.Domains.Products.ProductListModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Products Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <table id="products"></table>
    <div id="pager2"></div>

    <script language="javascript">
        $("#products").jqGrid(
        {
            url: '/Products/GetProducts',
            datatype: "json",
            colNames: ['Id', 'Name'],
            colModel: [
   		        { name: 'Id', index: 'Id', width: 300 },
                { name: 'Name', index: 'Name', width: 200 },
   	        ],
            rowNum: 10,
            rowList: [10, 50, 100],
            // pager: '#pager2',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Products",
            onSelectRow: function (id) {
                $.ajax(
                {
                    url: "/Products/GetProductDetails/" + id,
                    success: function (json) {
                        $("#Id").attr('value', function () { return json.rows[0].cell[0]; });
                        $("#Name").attr('value', function () { return json.rows[0].cell[1]; });
                        $("#StockCount").attr('value', function () { return json.rows[0].cell[2]; });
                    }
                });
            }
        });
        // $("#products").jqGrid('navGrid', '#pager2', { edit: false, add: false, del: false });
    </script>

    <%= Html.ActionLink("Add Product", "AddProduct") %>

</asp:Content>
