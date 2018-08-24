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
            React.createElement(PeriodSummary))
    }
}


ReactDOM.render(
  React.createElement(PeriodSummaryContainer),
  document.getElementById('header')
);