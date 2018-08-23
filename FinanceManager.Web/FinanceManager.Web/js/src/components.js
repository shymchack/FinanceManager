class PeriodSummaryHeader extends React.Component {
    render() {
        return React.createElement('div', { className: 'col-xs-12 text-center' },
            React.createElement('h1', null, `${this.props.periodTitle} Summary`));
    }
}

ReactDOM.render(
  React.createElement(PeriodSummaryHeader, { periodTitle: 'October 2018' }, null),
  document.getElementById('header')
);