import React from 'react';

import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { linkTo } from '@storybook/addon-links';

import Submitbutton from "../../../materialui/buttons/submitbutton"


storiesOf('Form', module)
  .add('SubmitButton', () => <Submitbutton />);

