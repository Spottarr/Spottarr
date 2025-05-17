<script setup lang="ts">
import Search from '@/components/Search.vue';
import SpotCategoryNav from '@/components/SpotCategoryNav.vue';
import UserInfo from '@/components/UserInfo.vue';
</script>

<template>
  <user-info />
  <search />
  <h2 class="text-lg mb-4">Filters</h2>
  <spot-category-nav />
</template>
