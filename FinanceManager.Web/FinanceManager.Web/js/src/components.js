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
                React.createElement(PeriodSummaryItem, { title: 'Incomes', periodBeginningValue: '1500', periodDifference: '2500', periodValue: '3500'}),
                React.createElement(PeriodSummaryItem, { title: 'Expenses', periodBeginningValue: '4500', periodDifference: '5500', periodValue: '6500' }),
                React.createElement(PeriodSummaryItem, { title: 'Period Balance', periodBeginningValue: '7500', periodDifference: '8500', periodValue: '9500' }),
                React.createElement(PeriodSummaryItem, { title: 'Total Balance', periodBeginningValue: '10500', periodDifference: '11500', periodValue: '12500' })));
    }
}

class PeriodSummaryContainer extends React.Component {
    render() {
        return React.createElement('div', null,
            React.createElement(PeriodSummaryHeader, { periodTitle: 'October 2018' }),
            React.createElement(PeriodSummary),
            React.createElement(MoneyAllocationItemsGrid))
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

ReactDOM.render(
  React.createElement(PeriodSummaryContainer),
  document.getElementById('root')
);