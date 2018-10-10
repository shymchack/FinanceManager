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
            React.createElement(MoneyAllocationItemsGrid, { moneyAllocationItems: this.props.moneyAllocationItems }),
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
            React.createElement('div', {className: 'to-bottom'}, 
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

class MoneyAllocationItemsGrid extends React.Component {
    render() {
        return React.createElement('div', {className: 'container', style: [{paddingTop: '20px'}] }, 
            React.createElement('div', {className:'row'}, 
                React.createElement('div', {className: 'col-sm-4'}, 'NameLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'TotalAmountLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'FinishDateLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'PaymentLeftLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'ALreadyPayedLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'CurrentPeriodEndAmountLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'CurrentPeriodBudgetedAmountLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'CurrentPeriodPayedAmountLabel'),
                React.createElement('div', {className: 'col-sm-1'}, 'CurrentPeriodPaymentLeftLabel')
            ),
            React.createElement(MoneyAllocationItem)
            );
    }
}

class MoneyAllocationItem extends React.Component {
    render() {
        return React.createElement('div', { className: 'row' },
                React.createElement('div', { className: 'col-sm-4' }, 'OC Volvo'),
                React.createElement('div', { className: 'col-sm-1' }, '3000'),
                React.createElement('div', { className: 'col-sm-1' }, '120'),
                React.createElement('div', { className: 'col-sm-1' }, '12323'),
                React.createElement('div', { className: 'col-sm-1' }, '124'),
                React.createElement('div', { className: 'col-sm-1' }, '1231'),
                React.createElement('div', { className: 'col-sm-1' }, '123'),
                React.createElement('div', { className: 'col-sm-1' }, '3123'),
                React.createElement('div', { className: 'col-sm-1' }, '231')
            )
    }
}

class NewMoneyAllocationItemForm extends React.Component {
    render() {
        return React.createElement('form', { action: '/HomeController/AddExpense', method: 'post' },
            React.createElement('label', { for: 'NewMoneyOperation_Name' }, 'Name'),
            React.createElement('input', { id: 'NewMoneyOperation_Name', name: 'NewMoneyOperation_Name', type: 'text' }),
            React.createElement('label', { for: 'NewMoneyOperation_Description' }, 'Description'),
            React.createElement('input', { id: 'NewMoneyOperation_Description', name: 'NewMoneyOperation_Description', type: 'text' }),
            React.createElement('label', { for: 'NewMoneyOperation_InitialAmount' }, 'Initial amount'),
            React.createElement('input', { id: 'NewMoneyOperation_InitialAmount', name: 'NewMoneyOperation_InitialAmount' }),
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
          moneyAllocationItems: model.OperationsModel
      }),
      document.getElementById('root')
    );
}