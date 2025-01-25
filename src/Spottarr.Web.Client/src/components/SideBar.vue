<script setup lang="ts">
import Search from '@/components/Search.vue';
import SpotCategoryNav from '@/components/SpotCategoryNav.vue';
import UserInfo from '@/components/UserInfo.vue';
</script>

<template>
  <UserInfo />
  <Search />
  <h2 class="text-lg mb-4">Filters</h2>
  <SpotCategoryNav />
</template>
