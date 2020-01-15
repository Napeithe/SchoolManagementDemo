app.table = app.module(app.table);
app.table = (function () {
    function _createTable(id) {
        let tableContainer = $(`#${id}`);

        tableContainer.addClass("card shadow mb-4");

        let titleElement = $(`<div class="card-header py-3">
                                    <h6 class="m-0 font-weight-bold text-primary title"></h6>
                                </div>`);
        let bodyElement = $(`<div class="card-body">
                                <div class="row">
                                    <div class="col-sm-12"> 
                                        <div class="table-responsive">
                                            <table id="table-${id}" class="table table-bordered table-striped" width="100%"></table>
                                        </div>
                                    </div>        
                                </div>
                            </div>`);

        tableContainer.append(titleElement);
        tableContainer.append(bodyElement);



        let table = {};
        table.selector = tableContainer;
        table.withColumns = _withColumns;
        table.withData = _withData;
        table.withTitle = _withTitle;
        table.build = _build;
        table.tableOptions = {
            language: app.tableLanguage,
            responsive: {
                details: true
            }
        };
        table.withClickRowAndGotoById = _withClickRowAndGotoById;
        table.rowClick = {
            isSet: false
        };

        return table;

    }

    function _withColumns(columns) {
        this.tableOptions.columns = columns;
        return this;
    }

    function _withClickRowAndGotoById(goToUrl) {
        this.rowClick.isSet = true;
        this.rowClick.url = goToUrl;
        return this;
    }

    function _withData(dataSet) {
        this.tableOptions.data = dataSet;
        return this;
    }

    function _withTitle(title) {
        this.selector.find(".card-header > .title").text(title);

        return this;
    }

    function _build() {
        let table = this.selector.find("table");
        let renderedTable = table.DataTable(this.tableOptions);
        if (this.rowClick.isSet) {
            app.rowClickAndGoToByIdEvent(renderedTable, this.rowClick.url);
        }
    }

    return {
        createTable: _createTable
    };
})();