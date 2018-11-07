class PeriodSummaryContainer extends React.Component {
    render() {
        return React.createElement('div', null,
            React.createElement(PeriodSummaryHeader, { periodTitle: this.props.periodTitle }),
            React.createElement(PeriodSummary, {
                currentTotalBalance: this.props.currentTotalBalance,
                periodBeginningTotalBalance: this.props.periodBeginningTotalBalance,
                totalBalanceDifference: this.props.totalBalanceDifference,
                currentPeriodBalance: this.props.currentPeriodBalance,
                periodBeginningPeriodBalance: this.props.periodBeginningPeriodBalance,
                periodBalanceDifference: this.props.periodBalanceDifference,
                periodBeginningPeriodExpensesAmount: this.props.periodBeginningPeriodExpensesAmount,
                currentPeriodExpensesAmount: this.props.currentPeriodExpensesAmount,
                periodExpensesDifference: this.props.periodExpensesDifference,
                periodBeginningPeriodIncomesAmount: this.props.periodBeginningPeriodIncomesAmount,
                currentPeriodIncomesAmount: this.props.currentPeriodIncomesAmount,
                periodIncomesDifference: this.props.periodIncomesDifference
            }),
            React.createElement(MoneyAllocationItemsModelGrid, { moneyAllocationItemsModel: this.props.moneyAllocationItemsModel }),
            React.createElement(NewMoneyAllocationItemForm))
    }
}

class PeriodSummaryHeader extends React.Component {
    render() {
        return React.createElement('div', { className: 'col-xs-12 text-center' },
            React.createElement('h1', null, `${this.props.periodTitle} Summary`));
    }
}

class PeriodSummaryItem extends React.Component {
    //TODO: make theese items heights equal.
    render() {
        return React.createElement('div', { className: 'col-3' },
            React.createElement('div', { className: 'to-bottom' },
                React.createElement('h2', { className: 'text-center' }, `${this.props.title}`),
                React.createElement('div', { className: 'row' },
                    React.createElement('div', { className: 'col-2 col-sm-4' }, `${this.props.periodBeginningValue}`),
                    React.createElement('div', { className: 'col-2 col-sm-4' }, `${this.props.periodDifference}`),
                    React.createElement('div', { className: 'col-2 col-sm-4' }, `${this.props.periodValue}`))));
    }
}

class PeriodSummary extends React.Component {
    render() {
        return React.createElement('div', { className: 'container' },
            React.createElement('div', { className: 'row row-conformity' },
                React.createElement(PeriodSummaryItem, { title: 'Incomes', periodBeginningValue: this.props.periodBeginningPeriodIncomesAmount, periodDifference: this.props.periodIncomesDifference, periodValue: this.props.currentPeriodIncomesAmount }),
                React.createElement(PeriodSummaryItem, { title: 'Expenses', periodBeginningValue: this.props.periodBeginningPeriodExpensesAmount, periodDifference: this.props.periodExpensesDifference, periodValue: this.props.currentPeriodExpensesAmount }),
                React.createElement(PeriodSummaryItem, { title: 'Period Balance', periodBeginningValue: this.props.periodBeginningPeriodBalance, periodDifference: this.props.periodBalanceDifference, periodValue: this.props.currentPeriodBalance }),
                React.createElement(PeriodSummaryItem, { title: 'Total Balance', periodBeginningValue: this.props.periodBeginningTotalBalance, periodDifference: this.props.totalBalanceDifference, periodValue: this.props.currentTotalBalance })));
    }
}

class MoneyAllocationItemsModelGrid extends React.Component {
    render() {

        var items = [];
        for (var i = 0; i < this.props.moneyAllocationItemsModel.PeriodOperations.length; i++) {
            items.push(React.createElement(MoneyAllocationItem, {
                key: 'MoneyAllocationItem_' + i, // TODO: this does not work at all
                Name: this.props.moneyAllocationItemsModel.PeriodOperations[i].Name,
                TotalAmount: this.props.moneyAllocationItemsModel.PeriodOperations[i].InitialAmount,
                FinishDate: this.props.moneyAllocationItemsModel.PeriodOperations[i].FinishDate,
                PaymentLeft: this.props.moneyAllocationItemsModel.PeriodOperations[i].CurrentAmount,
                AlreadyPayed: this.props.moneyAllocationItemsModel.PeriodOperations[i].AlreadyPayedAmount,
                CurrentPeriodEndAmount: this.props.moneyAllocationItemsModel.PeriodOperations[i].CurrentPeriodEndAmount,
                CurrentPeriodBudgetedAmount: this.props.moneyAllocationItemsModel.PeriodOperations[i].CurrentPeriodBudgetedAmount,
                CurrentPeriodPayedAmount: this.props.moneyAllocationItemsModel.PeriodOperations[i].CurrentPeriodPayedAmount,
                CurrentPeriodPaymentLeftLabel: this.props.moneyAllocationItemsModel.PeriodOperations[i].CurrentPeriodPaymentLeft
            }))
        };
        return React.createElement('div', { id: 'MoneyAllocationItemsModelGrid', className: 'container', style: [{ paddingTop: '20px' }] },
            React.createElement('div', { className: 'row' },
                React.createElement('div', { className: 'col-sm-4' }, this.props.moneyAllocationItemsModel.NameLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.TotalAmountLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.FinishDateLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.PaymentLeftLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.ALreadyPayedLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.CurrentPeriodEndAmountLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.CurrentPeriodBudgetedAmountLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.CurrentPeriodPayedAmountLabel),
                React.createElement('div', { className: 'col-sm-1' }, this.props.moneyAllocationItemsModel.CurrentPeriodPaymentLeftLabel)
            ),
            React.createElement(MoneyAllocationItems, { items: items })
    )}
}

class MoneyAllocationItems extends React.Component {
    render() {
        return this.props.items  ;
    }
}

class MoneyAllocationItem extends React.Component {
    render() {
        return React.createElement('div', { id: 'MoneyAllocationItem_' + this.props.Id, className: 'row' },
                React.createElement('div', { className: 'col-sm-4' }, this.props.Name),
                React.createElement('div', { className: 'col-sm-1' }, this.props.TotalAmount),
                React.createElement('div', { className: 'col-sm-1' }, moment(this.props.FinishDate).toDate().toLocaleDateString()),
                React.createElement('div', { className: 'col-sm-1' }, this.props.PaymentLeft),
                React.createElement('div', { className: 'col-sm-1' }, this.props.AlreadyPayed),
                React.createElement('div', { className: 'col-sm-1' }, this.props.CurrentPeriodEndAmount),
                React.createElement('div', { className: 'col-sm-1' }, this.props.CurrentPeriodBudgetedAmount),
                React.createElement('div', { className: 'col-sm-1' }, this.props.CurrentPeriodPayedAmount),
                React.createElement('div', { className: 'col-sm-1' }, this.props.CurrentPeriodPaymentLeftLabel),
                React.createElement('div', { className: 'col-sm-1' } )
            )
    }
}

class NewMoneyAllocationItemForm extends React.Component {
    render() {
        return React.createElement('form', { action: '/HomeController/AddExpense', method: 'post' },
            React.createElement('label', { for: 'NewMoneyOperation_Name' }, 'Name'),
            React.createElement('input', { id: 'NewMoneyOperation_Name', name: 'NewMoneyOperation_Name', type: 'text' }),
            React.createElement('label', { for: 'NewMoneyOperation_InitialAmount' }, 'Initial amount'),
            React.createElement('input', { id: 'NewMoneyOperation_InitialAmount', name: 'NewMoneyOperation_InitialAmount' }),
            React.createElement('label', { for: 'NewMoneyOperation_BeginningDate' }, 'BeginningDate'),
            React.createElement('input', { id: 'NewMoneyOperation_BeginningDate', name: 'NewMoneyOperation_BeginningDate', type: 'text' }),
            React.createElement('label', { for: 'NewMoneyOperation_EndDate' }, 'EndDate'),
            React.createElement('input', { id: 'NewMoneyOperation_EndDate', name: 'NewMoneyOperation_EndDate', type: 'text' }),
            React.createElement('label', { for: 'NewMoneyOperation_RepetitionUnitQuantity' }, 'Repetition unit quantity'),
            React.createElement('input', { id: 'NewMoneyOperation_RepetitionUnitQuantity', name: 'NewMoneyOperation_RepetitionUnitQuantity' }),
            React.createElement('label', { for: 'NewMoneyOperation_IsReal' }, 'Is real?'),
            React.createElement('input', { id: 'NewMoneyOperation_IsReal', name: 'NewMoneyOperation_IsReal', type: 'checkbox' }),
            React.createElement('button', { type: 'submit' }, 'SUBMIT'),
            React.createElement('input', { id: 'NewMoneyOperation_AccountID', name: 'NewMoneyOperation_AccountID', type: 'hidden' }));
    }
}

function renderPeriodSummaryViewModel(model) {
    ReactDOM.render(
      React.createElement(PeriodSummaryContainer, {
          periodTitle: model.PeriodTitle,
          currentTotalBalance: model.CurrentTotalBalance,
          periodBeginningTotalBalance: model.PeriodBeginningTotalBalance,
          totalBalanceDifference: model.TotalBalanceDifference,
          currentPeriodBalance: model.CurrentPeriodBalance,
          periodBeginningPeriodBalance: model.PeriodBeginningPeriodBalance,
          periodBalanceDifference: model.PeriodBalanceDifference,
          periodBeginningPeriodExpensesAmount: model.PeriodBeginningPeriodExpensesAmount,
          currentPeriodExpensesAmount: model.CurrentPeriodExpensesAmount,
          periodExpensesDifference: model.PeriodExpensesDifference,
          periodBeginningPeriodIncomesAmount: model.PeriodBeginningPeriodIncomesAmount,
          currentPeriodIncomesAmount: model.CurrentPeriodIncomesAmount,
          periodIncomesDifference: model.PeriodIncomesDifference,
          moneyAllocationItemsModel: model.OperationsModel
      }),
      document.getElementById('root')
    );
}