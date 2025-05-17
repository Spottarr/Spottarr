<script setup lang="ts">
import type { SortDirection } from '@tanstack/vue-table';
import { SortIcon, SortAscendingIcon, SortDescendingIcon } from 'mdi-vue3';

defineProps<{ isSorted: false | SortDirection }>();
</script>

<template>
  <sort-ascending-icon v-if="isSorted === 'asc'" class="size-5 fill-gray-400 dark:fill-slate-400" />
  <sort-descending-icon
    v-else-if="isSorted === 'desc'"
    class="size-5 fill-gray-400 dark:fill-slate-400"
  />
  <sort-icon v-else class="size-5 fill-gray-400 dark:fill-slate-400" />
</template>

<style scoped></style>
