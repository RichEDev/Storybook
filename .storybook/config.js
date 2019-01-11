import { configure } from '@storybook/react';

function loadStories() {
  require('../src/stories');
  require('../src/stories/form/buttons');
}

configure(loadStories, module);
