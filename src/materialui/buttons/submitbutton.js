import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';

const styles = theme => ({
  button: {
    margin: theme.spacing.unit,
  },
  input: {
    display: 'none',
  },
});

class dddd extends React.Component {
  render() {
    return (<div> hello </div>)
  }
}

class Submitbutton extends React.Component {
  constructor(props) {
    super(props);


    // This binding is necessary to make `this` work in the callback
    this.handleClick = this.handleClick.bind(this);
  }

  handleClick() {
    alert('You have clicked submit')
  }
    
  render() {
    return (
      <div>
        <Button variant="contained" color="primary" onClick={this.handleClick}  >
          Sumbmit
      </Button>
      </div>)

  }
}

export default Submitbutton;